using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wasalnyy.BLL.DTO.Trip;

namespace Wasalnyy.BLL.Enents
{
    public class TripEvents
    {
        public delegate void TripDel(TripDto dto);
        public event TripDel? TripRequested;
        public event TripDel? TripAccepted;
        public event TripDel? TripStarted;
        public event TripDel? TripEnded;
        public event TripDel? TripCanceled;
        public void FireTripRequested(TripDto dto)
        {
            TripRequested?.Invoke(dto);
        }
        public void FireTripAccepted(TripDto dto)
        {
            TripAccepted?.Invoke(dto);
        }
        public void FireTripStarted(TripDto dto)
        {
            TripStarted?.Invoke(dto);
        }
        public void FireTripEnded(TripDto dto)
        {
            TripEnded?.Invoke(dto);
        }
        public void FireTripCanceled(TripDto dto)
        {
            TripCanceled?.Invoke(dto);
        }
    }
}
