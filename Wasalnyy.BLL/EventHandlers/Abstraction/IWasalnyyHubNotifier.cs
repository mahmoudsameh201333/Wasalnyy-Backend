namespace Wasalnyy.BLL.EventHandlers.Abstraction
{
    public interface IWasalnyyHubNotifier
    {
        Task OnUserConnected(string userId, string connectionId);
        Task OnUserDisconnected(string connectionId);
    }
}
