namespace Wasalnyy.BLL.Service.Implementation
{
    public class PricingService : IPricingService
    {
        private readonly PricingSettings _settings;
        private readonly PricingServiceValidator _validator;

        public PricingService(PricingSettings settings, PricingServiceValidator validator)
        {
            _settings = settings;
            _validator = validator;
        }

        public async Task<double> CalculatePriceAsync(CalculatePriceDto dto)
        {
            await _validator.ValidateCalculatePrice(dto);

            var total = _settings.BaseFare
                                    + (dto.DistanceKm * _settings.PricePerKm)
                                    + (dto.DurationMinutes * _settings.PricePerMinute);

            return Math.Round(total * _settings.SurgeMultiplier, 2);
        }
    }
}
