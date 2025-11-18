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

        Task OnDriverStatusChangedToAvailable(string driverId, Guid zoneId);
        Task OnDriverZoneChanged(string driverId, Guid? oldZoneId, Guid newZoneId);
        Task OnDriverLocationUpdated(string driverId, Coordinates coordinates);
        Task OnDriverOutOfZone(string driverId);
        Task OnDriverStatusChangedToOffline(string driverId);
        //Task OnDriverStatusChangedToInTrip(string driverId, Guid tripId);

    }
}
