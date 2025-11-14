using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wasalnyy.DAL.Entities;

namespace Wasalnyy.BLL.Service.Abstraction
{
    public interface IRouteService
    {
        Task<(double distanceKm, double durationMinutes)> CalculateDistanceAndDurationAsync(Coordinates startCoordinates, Coordinates endCoordinates);
    }
}
