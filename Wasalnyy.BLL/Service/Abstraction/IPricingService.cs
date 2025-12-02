namespace Wasalnyy.BLL.Service.Abstraction
{
    public interface IPricingService
    {
        public Task<double> CalculatePriceAsync(CalculatePriceDto dto);
    }
}
