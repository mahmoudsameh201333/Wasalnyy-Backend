using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wasalnyy.BLL.DTO.Trip;
using Wasalnyy.DAL.Enum;

namespace Wasalnyy.BLL.Enents
{
    public class DriverEvents
    {
        public delegate Task DriverDel(string driverId);
        public delegate Task DriverStatusChangedToInTripDel(string driverId, Guid tripId);
        public delegate Task DriverLocationUpdatedDel(string driverId, Coordinates coordinate);
        public delegate Task DriverZoneDel(string driverId, Guid zoneId);
        public delegate Task DriverZoneChangedDel(string driverId, Guid? oldZoneId, Guid newZoneId);

        public event DriverDel? DriverStatusChangedToUnAvailable;
        public event DriverDel? DriverOutOfZone;
        public event DriverZoneDel? DriverStatusChangedToAvailable;

        public event DriverLocationUpdatedDel? DriverLocationUpdated;
        public event DriverZoneChangedDel? DriverZoneChanged;

        public void FireDriverStatusChangedToUnAvailable(string driverId)
        {
            DriverStatusChangedToUnAvailable?.Invoke(driverId).Wait();
        }
        public void FireDriverOutOfZone(string driverId)
        {
            DriverOutOfZone?.Invoke(driverId).Wait();
        }

        //public void FireDriverStatusChangedToInTrip(string driverId, Guid tripId)
        //{
        //    DriverStatusChangedToInTrip?.Invoke(driverId, tripId).Wait();
        //}

        public void FireDriverStatusChangedToAvailable(string driverId, Guid zoneId)
        {
            DriverStatusChangedToAvailable?.Invoke(driverId, zoneId).Wait();
        }

        public void FireDriverLocationUpdated(string driverId, Coordinates coordinate)
        {
            DriverLocationUpdated?.Invoke(driverId, coordinate).Wait();
        }
        public void FireDriverZoneChanged(string driverId, Guid? oldZoneId, Guid newZoneId)
        {
            DriverZoneChanged?.Invoke(driverId, oldZoneId, newZoneId).Wait();
        }
    }
}
