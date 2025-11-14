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
        void OnTripRequested(TripDto dto);
        void OnTripAccepted(TripDto dto);
        void OnTripStarted(TripDto dto);
        void OnTripEnded(TripDto dto);
        void OnTripCanceled(TripDto dto);
    }
}
