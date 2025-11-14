using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wasalnyy.DAL.Entities;

namespace Wasalnyy.BLL.Helper
{
    public static class ZoneHelper
    {
        public static (decimal MinLat, decimal MaxLat, decimal MinLng, decimal MaxLng) GetBoundingBoxAsync(List<Coordinates> coordinates)
        {
            return (coordinates.Min(c => c.Lat), coordinates.Max(c => c.Lat), coordinates.Min(c => c.Lng), coordinates.Max(c => c.Lng));
        }

        public static bool IsPointInPolygon(decimal lat, decimal lng, List<Coordinates> polygon)
        {
            bool inside = false;
            for (int i = 0, j = polygon.Count - 1; i < polygon.Count; j = i++)
            {
                if ((polygon[i].Lat > lat) != (polygon[j].Lat > lat) &&
                    (lng < (polygon[j].Lng - polygon[i].Lng) * (lat - polygon[i].Lat) /
                     (polygon[j].Lat - polygon[i].Lat) + polygon[i].Lng))
                {
                    inside = !inside;
                }
            }
            return inside;
        }
    }
}
