using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wasalnyy.BLL.DTO.Trip;
using Wasalnyy.BLL.EventHandlers.Abstraction;

namespace Wasalnyy.BLL.EventHandlers.Implementation
{
    public class TripNotifier : ITripNotifier
    {
        public void OnTripAccepted(TripDto dto)
        {
            throw new NotImplementedException();
        }

        public void OnTripCanceled(TripDto dto)
        {
            throw new NotImplementedException();
        }

        public void OnTripEnded(TripDto dto)
        {
            throw new NotImplementedException();
        }

        public void OnTripRequested(TripDto dto)
        {
            throw new NotImplementedException();
        }

        public void OnTripStarted(TripDto dto)
        {
            throw new NotImplementedException();
        }
    }
}
