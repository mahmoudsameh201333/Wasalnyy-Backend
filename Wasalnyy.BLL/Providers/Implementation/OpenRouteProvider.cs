namespace Wasalnyy.BLL.Providers.Implementation
{
    public class OpenRouteProvider : IRouteProvider
    {
        private readonly HttpClient _httpClient;
        private readonly OpenRouteSettings _settings;

        public OpenRouteProvider(HttpClient http, OpenRouteSettings settings)
        {
            _httpClient = http;
            _settings = settings;
        }

        public async Task<(double distanceKm, double durationMinutes)?> GetRouteAsync(Coordinates start, Coordinates end)
        {
            try
            {
                var url = "https://api.openrouteservice.org/v2/directions/driving-car/geojson";

                var body = new
                {
                    coordinates = new[]
                    {
                    new[] { start.Lng, start.Lat },
                    new[] { end.Lng, end.Lat }
                }
                };

                using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(10));

                var request = new HttpRequestMessage(HttpMethod.Post, url)
                {
                    Content = JsonContent.Create(body)
                };

                // ✔ Correct: Raw key (NO Bearer)
                request.Headers.TryAddWithoutValidation("Authorization", _settings.ApiKey);

                // Optional but recommended:
                request.Headers.TryAddWithoutValidation(
                    "Accept",
                    "application/json, application/geo+json, application/gpx+xml, img/png; charset=utf-8"
                );

                var response = await _httpClient.SendAsync(request, cts.Token);

                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    Console.WriteLine("ORS ERROR: " + error);
                    return null;
                }

                var jsonString = await response.Content.ReadAsStringAsync(cts.Token);
                var json = JsonDocument.Parse(jsonString);

                var route = json.RootElement
                    .GetProperty("features")[0]
                    .GetProperty("properties")
                    .GetProperty("segments")[0];

                double distanceKm = route.GetProperty("distance").GetDouble() / 1000.0;
                double durationMinutes = route.GetProperty("duration").GetDouble() / 60.0;

                return (distanceKm, durationMinutes);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: " + ex.Message);
                return null;
            }
        }
    }


}