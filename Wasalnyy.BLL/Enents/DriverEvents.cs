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
        public delegate void DriverStatusChangedDel(string driverId, DriverStatus driverStatus);
        public delegate void DriverLocationUpdatedDel(string driverId, decimal lng, decimal lat);

        public event DriverStatusChangedDel? DriverStatusChanged;
        public event DriverLocationUpdatedDel? DriverLocationUpdated;

        public void FireDriverStatusChanged(string driverId, DriverStatus driverStatus) 
        { 
            DriverStatusChanged?.Invoke(driverId, driverStatus); 
        }

        public void FireDriverLocationUpdated(string driverId, decimal lng, decimal lat)
        {
            DriverLocationUpdated?.Invoke(driverId, lng, lat);
        }
    }
}
