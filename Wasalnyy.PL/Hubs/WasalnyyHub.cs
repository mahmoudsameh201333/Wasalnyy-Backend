using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace Wasalnyy.PL.Hubs
{
    [Authorize(Roles = "Driver,Rider")]
    public class WasalnyyHub : Hub
    {
        private readonly IWasalnyyHubConnectionService _connectionService;
        public WasalnyyHub(IWasalnyyHubConnectionService connectionService)
        {
            _connectionService = connectionService;
        }

        public override  Task OnConnectedAsync()
        {
            var currentUserId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            string conId = Context.ConnectionId;

            if (string.IsNullOrEmpty(currentUserId) || string.IsNullOrEmpty(conId))
                throw new UnauthorizedAccessException();


             _connectionService.CreateAsync(new DAL.Entities.WasalnyyHubConnection { SignalRConnectionId = conId, UserId = currentUserId }).Wait();

            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception? exception)
        {
            string conId = Context.ConnectionId;
            if (!string.IsNullOrEmpty(conId))
                _connectionService.DeleteAsync(conId).Wait();
            return base.OnDisconnectedAsync(exception);
        }
    }
}
