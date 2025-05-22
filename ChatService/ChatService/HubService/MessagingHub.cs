using ChatService.DatabaseContext;
using ChatService.Entities;
using ChatService.TrackerRedis;
using Microsoft.AspNetCore.SignalR;


namespace ChatService.HubService
{
    public class MessagingHub : Hub
    {
        private readonly AppDbContext _db;

        private readonly PresenceService _presence;

        public MessagingHub(AppDbContext db, PresenceService presence)
        {
            _db = db;
            _presence = presence;
        }

        public override async Task OnConnectedAsync()
        {
            var userId = Context.UserIdentifier!;
            await _presence.SetOnline(userId);
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var userId = Context.UserIdentifier!;
            await _presence.SetOffline(userId);
            await base.OnDisconnectedAsync(exception);
        }

        public async Task SendMessage(string toUserId, string messageText)
        {
            var message = new Message
            {
                SenderId = Context.UserIdentifier!,
                ReceiverId = toUserId,
                Content = messageText
            };

            _db.Messages.Add(message);
            await _db.SaveChangesAsync();

            // Gửi tới người nhận
            await Clients.User(toUserId).SendAsync("ReceiveMessage", new
            {
                From = Guid.Parse(Context.UserIdentifier!),
                Content = message.Content,
                Timestamp = message.Timestamp
            });

            // Echo lại sender (nếu cần)
            await Clients.User(Context.UserIdentifier!).SendAsync("MessageSent", new
            {
                To = toUserId,
                Content = message.Content,
                Timestamp = message.Timestamp
            });
        }
    }

}
