namespace Wasalnyy.BLL.EventHandlers.Abstraction
{
    public interface IDriverNotifier
    {
        Task OnDriverStatusChangedToAvailable(string driverId, Guid zoneId);
        Task OnDriverZoneChanged(string driverId, Guid? oldZoneId, Guid newZoneId);
        Task OnDriverLocationUpdated(string driverId, Coordinates coordinates);
        Task OnDriverOutOfZone(string driverId);
        Task OnDriverStatusChangedToUnAvailable(string driverId);
    }
}
