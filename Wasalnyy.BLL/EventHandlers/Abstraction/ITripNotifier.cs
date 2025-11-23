using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wasalnyy.BLL.DTO.Trip;

namespace Wasalnyy.BLL.EventHandlers.Abstraction
{
    public interface ITripNotifier
    {
        Task OnTripRequested(TripDto dto);
        Task OnTripAccepted(TripDto dto);
        Task OnTripStarted(TripDto dto);
        Task OnTripEnded(TripDto dto);
        Task OnTripCanceled(TripDto dto);
        Task OnTripConfirmed(TripDto dto);
    }
}
