namespace Wasalnyy.BLL.EventHandlers.Abstraction
{
    public interface ITripNotifier
    {
        Task OnTripRequested(TripDto dto);
        Task OnTripAccepted(TripDto dto);
        Task OnTripStarted(TripDto dto);
        Task OnTripEnded(TripDto dto);
        Task OnTripCanceled(TripDto dto, TripStatus oldStatus, CashCancelationFees? cashCancelationFees);
        Task OnTripConfirmed(TripDto dto);
    }
}
