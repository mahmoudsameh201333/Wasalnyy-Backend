namespace Wasalnyy.BLL.Providers.Abstraction
{
    public interface IRouteProvider
    {
        Task<(double distanceKm, double durationMinutes)?> GetRouteAsync(Coordinates start, Coordinates end);
    }
}
