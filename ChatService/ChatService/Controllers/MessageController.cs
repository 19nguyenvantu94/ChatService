using ChatService.DatabaseContext;
using ChatService.Entities;
using ChatService.TrackerRedis;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Minio;
using Minio.DataModel.Args;
using System;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;

namespace ChatService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MessagesController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly PresenceService _presence;
    private readonly IMinioClient _minioClient;    

    public MessagesController(AppDbContext db, PresenceService presence, IMinioClient minioClient)
    {
        _db = db;
        _presence = presence;
        _minioClient = minioClient;
    }

    [HttpPost("send")]
    public async Task<IActionResult> SendMessage(
    [FromForm] string fromUserId,
    [FromForm] string toUserId,
    [FromForm] string? text,
    [FromForm] string? emojis, // JSON array string
    [FromForm] List<IFormFile>? files)
    {
        var attachments = new List<MessageAttachment>();
        var emojiList = new List<MessageEmoji>();

        // Parse emoji JSON (ex: ["🔥", "👍"])
        if (!string.IsNullOrEmpty(emojis))
        {
            var parsed = JsonSerializer.Deserialize<List<string>>(emojis);
            emojiList = parsed?.Select(e => new MessageEmoji { Emoji = e }).ToList() ?? new();
        }

        // Upload files to R2
        if (files != null)
        {
            foreach (var file in files)
            {
                var ext = Path.GetExtension(file.FileName).ToLower();
                var type = ext is ".mp3" or ".wav" ? "audio" : "image";

                var objectName = $"messages/{Guid.NewGuid()}{ext}";
                var stream = file.OpenReadStream();

                await _minioClient.PutObjectAsync(new PutObjectArgs()
                    .WithBucket("chatfiles")
                    .WithObject(objectName)
                    .WithStreamData(stream)
                    .WithObjectSize(file.Length)
                    .WithContentType(file.ContentType));

                var fileUrl = $"https://pub-7c8f0bae44cc46298416c9cd9f502349.r2.dev/chat-files/{objectName}";
                attachments.Add(new MessageAttachment { FileUrl = fileUrl, Type = type });
            }
        }

        // Save to DB
        var message = new Message
        {
            SenderId = fromUserId,
            ReceiverId = toUserId,
            Content = text,
            Emojis = emojiList,
            Attachments = attachments
        };

        _db.Messages.Add(message);
        await _db.SaveChangesAsync();

        return Ok(new
        {
            message.Id,
            message.Content,
            Emojis = emojiList.Select(e => e.Emoji),
            Attachments = attachments.Select(a => new { a.Type, a.FileUrl })
        });
    }


    [HttpGet("history")]
    [Authorize]
    public async Task<IActionResult> GetHistory([FromQuery] string withUserId)
    {
        var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        var messages = await _db.Messages
            .Where(m =>
                (m.SenderId.ToString() == currentUserId && m.ReceiverId.ToString() == withUserId) ||
                (m.SenderId.ToString() == withUserId && m.ReceiverId.ToString() == currentUserId))
            .OrderBy(m => m.Timestamp)
            .ToListAsync();

        return Ok(messages);
    }

    [HttpPost("mark-read")]
    [Authorize]
    public async Task<IActionResult> MarkMessagesAsRead([FromBody] string fromUserId)
    {
        var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        var unreadMessages = await _db.Messages
            .Where(m => m.SenderId.ToString() == fromUserId && m.ReceiverId.ToString() == currentUserId && !m.IsRead)
            .ToListAsync();

        unreadMessages.ForEach(m => m.IsRead = true);
        await _db.SaveChangesAsync();

        return Ok();
    }

    [HttpGet("status/{userId}")]
    [Authorize]
    public async Task<IActionResult> GetStatus(string userId)
    {
        if (await _presence.IsOnline(userId))
            return Ok(new { status = "online" });

        var lastSeen = _presence.GetLastSeen(userId);
        return Ok(new { status = "offline", lastSeen });
    }

}
