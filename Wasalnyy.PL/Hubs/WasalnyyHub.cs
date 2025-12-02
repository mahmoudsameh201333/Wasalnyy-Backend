namespace Wasalnyy.PL.Hubs
{
    [Authorize(Roles = "Driver,Rider")]
    public class WasalnyyHub : Hub
    {
        private readonly WasalnyyHubEvents _hubEvents;
        public WasalnyyHub(WasalnyyHubEvents hubEvents)
        {
            _hubEvents = hubEvents;
        }

        public override async Task OnConnectedAsync()
        {
            var currentUserId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            string conId = Context.ConnectionId;

            if (string.IsNullOrEmpty(currentUserId) || string.IsNullOrEmpty(conId))
                throw new UnauthorizedAccessException();

            await base.OnConnectedAsync();

            await _hubEvents.FireUserConnectedAsync(currentUserId, conId);
            Console.WriteLine("user " + currentUserId + " connected");
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            string conId = Context.ConnectionId;

            if (!string.IsNullOrEmpty(conId))
                await _hubEvents.FireOnUserDisconnectedAsync(conId);

            await base.OnDisconnectedAsync(exception);
        }
    }
}
