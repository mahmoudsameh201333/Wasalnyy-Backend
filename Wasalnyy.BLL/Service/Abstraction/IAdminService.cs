using Wasalnyy.BLL.DTO.Update;
using Wasalnyy.DAL.Entities;
using Wasalnyy.DAL.Enum;

namespace Wasalnyy.BLL.Service.Abstraction
{
    public interface IAdminService
    {
        // =======================
        // Riders
        // =======================
        Task<IEnumerable<Rider>> GetAllRidersAsync();
        Task<Rider?> GetRiderByPhoneAsync(string phone);
        Task<bool> DeleteRiderAsync(string riderId);
        Task<int> GetRiderTripCountAsync(string riderId);

        // NEW:
        Task<IEnumerable<Trip>> GetRiderTripsAsync(string riderId);


        // =======================
        // Drivers
        // =======================
        Task<IEnumerable<Driver>> GetAllDriversAsync();
        Task<Driver?> GetDriverByLicenseAsync(string license);
        Task<bool> UpdateDriverAsync(string id, UpdateDriver dto);
        Task<int> GetDriverTripCountAsync(string driverId);
        

        // NEW:
        Task<IEnumerable<Trip>> GetDriverTripsAsync(string driverId);


        // =======================
        // Trips
        // =======================
        
        Task<Trip?> GetTripByIdAsync(Guid id);

        Task<IEnumerable<Trip>> GetTripsByStatusAsync(TripStatus status);
        

       


        // =======================
        // Dashboard / Reports
        // =======================
        Task<int> GetTotalTripsAsync();
        Task<int> GetTotalDriversAsync();
        Task<int> GetTotalRidersAsync();

        
        Task<decimal> GetRevenueAsync(DateTime from, DateTime to);

        Task SuspendAccountDriver(string lic);
        Task SuspendAccountRider(string id);
    }
}
