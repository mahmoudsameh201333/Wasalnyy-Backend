namespace Wasalnyy.BLL.Helper
{
    public static class GeoMath
    {
        public static (double distanceKm, double durationMinutes)
            CalculateFallbackRoute(
                Coordinates a,
                Coordinates b,
                double roadFactor = 1.55,
                double avgSpeedKmH = 60,
                double trafficRatio = 1.1)
        {
            double straightDistance = HaversineDistanceKm(a, b);
            double distanceKm = straightDistance * roadFactor;
            double durationMinutes = (distanceKm / avgSpeedKmH) * 60.0 * trafficRatio;
            return (distanceKm, durationMinutes);
        }

        public static double HaversineDistanceKm(Coordinates a, Coordinates b)
        {
            double R = 6371;

            double dLat = DegreesToRadians((double)(b.Lat - a.Lat));
            double dLng = DegreesToRadians((double)(b.Lng - a.Lng));

            double lat1 = DegreesToRadians((double)a.Lat);
            double lat2 = DegreesToRadians((double)b.Lat);

            double h = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                       Math.Cos(lat1) * Math.Cos(lat2) *
                       Math.Sin(dLng / 2) * Math.Sin(dLng / 2);

            return R * 2 * Math.Asin(Math.Sqrt(h));
        }

        private static double DegreesToRadians(double deg)
        { 
           return deg* Math.PI / 180.0;
        }
    }

}