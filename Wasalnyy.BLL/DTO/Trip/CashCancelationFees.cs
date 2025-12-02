namespace Wasalnyy.BLL.DTO.Trip
{
    public class CashCancelationFees
    {
        public double DistanceTraveledCost { get; set; }
        public double Fees { get; set; }
        public double Total { get => DistanceTraveledCost + Fees; }
    }
}
