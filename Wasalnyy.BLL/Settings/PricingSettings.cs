namespace Wasalnyy.BLL.Settings
{
    public class PricingSettings
    {
        public double BaseFare { get; set; } = 10;
        public double PricePerKm { get; set; } = 3;
        public double PricePerMinute { get; set; } = 0.5;
        public double SurgeMultiplier { get; set; } = 1;
    }
}
