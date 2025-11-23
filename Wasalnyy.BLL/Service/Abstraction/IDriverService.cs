using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wasalnyy.BLL.DTO;
using Wasalnyy.BLL.DTO.Driver;
using Wasalnyy.BLL.DTO.Rider;
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
        Task<bool> UpdateDriverInfo(string id, DriverUpdateDto driverUpdate);

        //homepage info for  driver
        // Driver home page info
        Task<string> DriverNameAsync(string driverId);
        Task<string?> DriverProfileImageAsync(string driverId);
        Task<DriverStatus> GetDriverStatusAsync(string driverId); // Offline, Available, InTrip
        Task<int> GetTotalCompletedTripsAsync(string driverId);
        Task<decimal> GetDriverRatingAsync(string driverId);
        Task<VehicleDto?> GetDriverVehicleInfoAsync(string driverId);
        Task<bool> IsDriverSuspendedAsync(string driverId);
        Task<decimal> GetDriverWalletBalanceAsync(string driverId);



    }
}
