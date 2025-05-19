using ChatService.DatabaseContext;
using ChatService.TrackerRedis;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ChatService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MessagesController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly PresenceService _presence;

    public MessagesController(AppDbContext db, PresenceService presence)
    {
        _db = db;
        _presence = presence;
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
