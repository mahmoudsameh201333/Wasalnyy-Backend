namespace Wasalnyy.BLL.Enents
{
    public class DriverEvents
    {
        public event Func<string, Task>? DriverStatusChangedToUnAvailable;
        public event Func<string, Task>? DriverOutOfZone;
        public event Func<string, Guid, Task>? DriverStatusChangedToAvailable;

        public event Func<string, Coordinates, Task>? DriverLocationUpdated;
        public event Func<string, Guid?, Guid, Task>? DriverZoneChanged;

        public async Task FireDriverStatusChangedToUnAvailable(string driverId)
        {
            if(DriverStatusChangedToUnAvailable != null)
                await DriverStatusChangedToUnAvailable.Invoke(driverId);
        }
        public async Task FireDriverOutOfZone(string driverId)
        {
            if(DriverOutOfZone != null)
                await DriverOutOfZone.Invoke(driverId);
        }
        public async Task FireDriverStatusChangedToAvailable(string driverId, Guid zoneId)
        {
            if(DriverStatusChangedToAvailable != null)
                await DriverStatusChangedToAvailable.Invoke(driverId, zoneId);
        }

        public async Task FireDriverLocationUpdated(string driverId, Coordinates coordinate)
        {
            if(DriverLocationUpdated != null)
                await DriverLocationUpdated.Invoke(driverId, coordinate);
        }
        public async Task FireDriverZoneChanged(string driverId, Guid? oldZoneId, Guid newZoneId)
        {
            if(DriverZoneChanged != null)
                await DriverZoneChanged.Invoke(driverId, oldZoneId, newZoneId);
        }
    }
}
