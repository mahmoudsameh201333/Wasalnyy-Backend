namespace Wasalnyy.BLL.Providers.Implementation
{
    public class OsrmRouteProvider : IRouteProvider
    {
        private readonly HttpClient _httpClient;

        public OsrmRouteProvider(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        public async Task<(double distanceKm, double durationMinutes)?> GetRouteAsync(Coordinates start, Coordinates end)
        {
            try
            {
                var url = $"https://router.project-osrm.org/route/v1/driving/{start.Lng},{start.Lat};{end.Lng},{end.Lat}?overview=false";

                using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(3));
                var response = await _httpClient.GetStringAsync(url, cts.Token);

                var json = JsonDocument.Parse(response);

                var route = json.RootElement.GetProperty("routes")[0];  

                double distanceKm = route.GetProperty("distance").GetDouble() / 1000.0;
                double durationMinutes = route.GetProperty("duration").GetDouble() / 60.0;

                return (distanceKm, durationMinutes);
            }
            catch 
            {
                return null;
            }
        }
    }
}
