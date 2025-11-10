using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wasalnyy.DAL.Enum;

namespace Wasalnyy.BLL.EventHandlers.Abstraction
{
    public interface IDriverNotifier
    {
        void OnDriverStatusChanged(string driverId, DriverStatus driverStatus);
        void OnDriverLocationUpdated(string driverId, decimal lng, decimal lat);
    }
}
