namespace Wasalnyy.BLL.Validators
{
    public class PricingServiceValidator
    {
        public async Task ValidateCalculatePrice(CalculatePriceDto dto)
        {
            ArgumentNullException.ThrowIfNull(dto);

            if(dto.DurationMinutes < 0 || dto.DistanceKm < 0)
                throw new ArgumentException();
        }
    }
}
