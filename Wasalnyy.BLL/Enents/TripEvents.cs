namespace Wasalnyy.BLL.Enents
{
    public class TripEvents
    {
        public event Func<TripDto, Task>? TripRequested;
        public event Func<TripDto, Task>? TripAccepted;
        public event Func<TripDto, Task>? TripConfirmed;
        public event Func<TripDto, Task>? TripStarted;
        public event Func<TripDto, Task>? TripEnded;
        public event Func<TripDto, TripStatus, CashCancelationFees?, Task>? TripCanceled;
        public async Task FireTripRequested(TripDto dto)
        {
            if (TripRequested != null) 
                await TripRequested.Invoke(dto);
        }
        public async Task FireTripAccepted(TripDto dto)
        {
            if (TripAccepted != null)
                await TripAccepted.Invoke(dto);
        }
        public async Task FireTripStarted(TripDto dto)
        {
            if (TripStarted != null)
                await TripStarted.Invoke(dto);
        }
        public async Task FireTripEnded(TripDto dto)
        {
            if (TripEnded != null)
                await TripEnded.Invoke(dto);
        }
        public async Task FireTripCanceled(TripDto dto, TripStatus oldStatus , CashCancelationFees? cashCancelationFees)
        {
            if (TripCanceled != null)
                await TripCanceled.Invoke(dto, oldStatus, cashCancelationFees);
        }
        public async Task FireTripConfirmed(TripDto dto)
        {
            if (TripConfirmed != null)
                await TripConfirmed.Invoke(dto);
        }
    }
}
