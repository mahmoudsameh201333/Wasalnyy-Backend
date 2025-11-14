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
        public delegate void DriverStatusChangedDel(string driverId);
        public delegate void DriverStatusChangedToInTripDel(string driverId, Guid tripId);
        public delegate void DriverLocationUpdatedDel(string driverId, decimal lng, decimal lat);
        public delegate void DriverZoneChangedDel(string driverId, Guid zoneId);

        public event DriverStatusChangedDel? DriverStatusChangedToOffline;
        public event DriverStatusChangedToInTripDel? DriverStatusChangedToInTrip;
        public event DriverStatusChangedDel? DriverStatusChangedToAvailable;

        public event DriverLocationUpdatedDel? DriverLocationUpdated;
        public event DriverZoneChangedDel? DriverZoneChanged;

        public void FireDriverStatusChangedToOffline(string driverId)
        {
            DriverStatusChangedToOffline?.Invoke(driverId);
        }

        public void FireDriverStatusChangedToInTrip(string driverId, Guid tripId)
        {
            DriverStatusChangedToInTrip?.Invoke(driverId, tripId);
        }

        public void FireDriverStatusChangedToAvailable(string driverId)
        {
            DriverStatusChangedToAvailable?.Invoke(driverId);
        }

        public void FireDriverLocationUpdated(string driverId, decimal lng, decimal lat)
        {
            DriverLocationUpdated?.Invoke(driverId, lng, lat);
        }
        public void FireDriverZoneChanged(string driverId, Guid zoneId)
        {
            DriverZoneChanged?.Invoke(driverId, zoneId);
        }
    }
}
