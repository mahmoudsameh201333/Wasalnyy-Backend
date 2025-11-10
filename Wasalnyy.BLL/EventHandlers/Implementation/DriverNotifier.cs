using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wasalnyy.BLL.EventHandlers.Abstraction;
using Wasalnyy.DAL.Enum;

namespace Wasalnyy.BLL.EventHandlers.Implementation
{
    public class DriverNotifier : IDriverNotifier
    {
        public void OnDriverLocationUpdated(string driverId, decimal lng, decimal lat)
        {
            throw new NotImplementedException();
        }

        public void OnDriverStatusChanged(string driverId, DriverStatus driverStatus)
        {
            throw new NotImplementedException();
        }
    }
}
