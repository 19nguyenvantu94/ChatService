using ChatService.TrackerRedis;
using Microsoft.AspNetCore.SignalR;

namespace ChatService.HubService
{
    public class CallHub : Hub
    {
        private readonly PresenceService _presence;
        public CallHub(PresenceService presence)
        {
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

        public async Task SendOffer(string toUserId, string sdp)
            => await Clients.User(toUserId).SendAsync("ReceiveOffer", Context.UserIdentifier, sdp);

        public async Task SendAnswer(string toUserId, string sdp)
            => await Clients.User(toUserId).SendAsync("ReceiveAnswer", Context.UserIdentifier, sdp);

        public async Task SendIceCandidate(string toUserId, string candidate)
            => await Clients.User(toUserId).SendAsync("ReceiveIceCandidate", Context.UserIdentifier, candidate);
    }
}
