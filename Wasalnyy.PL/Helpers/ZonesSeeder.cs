using Wasalnyy.BLL.DTO.Zone;
using Wasalnyy.BLL.Service.Implementation;
namespace Wasalnyy.DAL.Helpers
{
    public static class ZonesSeeder
    {
        public static async Task SeedZonesAsync(
        IZoneService zoneService,
        Coordinates topLeft,
        Coordinates bottomRight)
        {
            const double zoneSizeKm = 3.0;

            double minLat = (double)bottomRight.Lat;
            double maxLat = (double)topLeft.Lat;

            double minLng = (double)topLeft.Lng;
            double maxLng = (double)bottomRight.Lng;

            double latStep = GeoConvert.KmToLatDegrees(zoneSizeKm);

            double midLat = (maxLat + minLat) / 2.0;
            double lngStep = GeoConvert.KmToLngDegrees(zoneSizeKm, midLat);

            for (double lat = minLat; lat < maxLat; lat += latStep)
            {
                double nextLat = Math.Min(lat + latStep, maxLat);

                for (double lng = minLng; lng < maxLng; lng += lngStep)
                {
                    double nextLng = Math.Min(lng + lngStep, maxLng);

                    var zoneCoordinates = new List<Coordinates>
                    {
                        new Coordinates { Lat = (decimal)lat,      Lng = (decimal)lng      }, // bottom-left
                        new Coordinates { Lat = (decimal)lat,      Lng = (decimal)nextLng  }, // bottom-right
                        new Coordinates { Lat = (decimal)nextLat,  Lng = (decimal)nextLng  }, // top-right
                        new Coordinates { Lat = (decimal)nextLat,  Lng = (decimal)lng      }  // top-left
                    };

                    var dto = new CreateZoneDto
                    {
                        Coordinates = zoneCoordinates
                    };

                    await zoneService.CreateZoneAsync(dto);
                }
            }
        }
    }
}
public static class GeoConvert
{
    private const double EarthKmPerLatDegree = 111.32;

    public static double KmToLatDegrees(double km)
        => km / EarthKmPerLatDegree;

    public static double KmToLngDegrees(double km, double latitudeDeg)
    {
        double latitudeRad = latitudeDeg * Math.PI / 180;
        return km / (EarthKmPerLatDegree * Math.Cos(latitudeRad));
    }
}