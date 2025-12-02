namespace Wasalnyy.BLL.Enents
{
    public class WasalnyyHubEvents
    {
        public event Func<string, string, Task>? UserConnected;
        public event Func<string, Task>? UserDisconnected;

        public async Task FireUserConnectedAsync(string userId, string connectionId)
        {
            if(UserConnected != null)
                await UserConnected.Invoke(userId, connectionId);
        }

        public async Task FireOnUserDisconnectedAsync(string connectionId)
        {
            if(UserDisconnected != null)
                await UserDisconnected.Invoke(connectionId);
        }
    }
}
