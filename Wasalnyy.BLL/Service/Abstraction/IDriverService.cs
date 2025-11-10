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
        Task ChangeStatusAsync(string driverId, DriverStatus status);
        Task UpdateLocationAsync(string driverId, Coordinate coordinate);
        Task<ReturnDriver?> GetByIdAsync(string id);
        Task<IEnumerable<ReturnDriver>> GetAvailableDriversByZoneAsync(Guid zoneId);
    }
}
