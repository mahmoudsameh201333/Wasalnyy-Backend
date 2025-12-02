namespace Wasalnyy.BLL.Service.Implementation
{
    public class RouteService : IRouteService
    {
        private readonly IEnumerable<IRouteProvider> _providers;
        private readonly RouteServiceValidator _validator;

        public RouteService(IEnumerable<IRouteProvider> providers, RouteServiceValidator validator)
        {
            _providers = providers;
            _validator = validator;
        }
        public async Task<(double distanceKm, double durationMinutes)> CalculateDistanceAndDurationAsync(Coordinates start, Coordinates end)
        {
            await _validator.ValidateCalculateDistanceAndDuration(start, end);

            foreach (var provider in _providers)
            {
                var result = await provider.GetRouteAsync(start, end);
                if (result != null)
                    return result.Value;
            }

            (double distanceKm, double durationMinutes ) = GeoMath.CalculateFallbackRoute(start, end);

            return GeoMath.CalculateFallbackRoute(start, end);
        }
    }
}