using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wasalnyy.BLL.DTO.Driver;
using Wasalnyy.DAL.Entities;
using Wasalnyy.DAL.Enum;

namespace Wasalnyy.BLL.Service.Abstraction
{
    public interface IDriverService
    {
        Task SetDriverUnAvailableAsync(string driverId);
        Task SetDriverInTripAsync(string driverId);
        Task SetDriverAvailableAsync(string driverId, Coordinates coordinates);
        Task UpdateLocationAsync(string driverId, Coordinates coordinate);
        Task<ReturnDriverDto?> GetByIdAsync(string id);
        Task<IEnumerable<ReturnDriverDto>> GetAvailableDriversByZoneAsync(Guid zoneId);
    }
}
