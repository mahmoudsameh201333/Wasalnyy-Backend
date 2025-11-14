using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Wasalnyy.BLL.Service.Abstraction;
using Wasalnyy.DAL.Entities;

namespace Wasalnyy.BLL.Service.Implementation
{
    public class RouteService : IRouteService
    {
        private readonly HttpClient _httpClient;

        public RouteService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        public async Task<(double distanceKm, double durationMinutes)> CalculateDistanceAndDurationAsync(Coordinates start, Coordinates end)
        {
            var url = $"https://router.project-osrm.org/route/v1/driving/{start.Lng},{start.Lat};{end.Lng},{end.Lat}?overview=false";

            var response = await _httpClient.GetStringAsync(url);
            var json = JsonDocument.Parse(response);

            var route = json.RootElement.GetProperty("routes")[0];
            var distanceMeters = route.GetProperty("distance").GetDouble();
            var durationSeconds = route.GetProperty("duration").GetDouble();

            double distanceKm = distanceMeters / 1000.0;
            double durationMinutes = durationSeconds / 60.0;

            return (distanceKm, durationMinutes);
        }
    }
}
