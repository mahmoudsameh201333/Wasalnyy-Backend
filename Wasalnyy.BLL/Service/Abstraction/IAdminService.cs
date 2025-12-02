using Wasalnyy.BLL.DTO.Update;
using Wasalnyy.DAL.Entities;
using Wasalnyy.DAL.Enum;

namespace Wasalnyy.BLL.Service.Abstraction
{
    public interface IAdminService
    {
        
        // Riders
        
        Task<IEnumerable<Rider>> GetAllRidersAsync();
        Task<double> GetRidersCount();
        Task<Rider?> GetRiderByPhoneAsync(string phone);
      
        Task<int> GetRiderTripCountAsync(string riderId);

        
        Task<IEnumerable<Trip>> GetRiderTripsAsync(string riderId);
        Task<IEnumerable<Trip>> GetRiderTripsAsyncByphone(string phonenum);
        Task<IEnumerable<Complaint>> GetRiderComplainsByPhoneAsync(string phonenum);
        Task<Complaint> GetRiderComplainByComplainsIdAsync(Guid id);
        


        // Drivers

        Task<IEnumerable<Driver>> GetAllDriversAsync();
        Task<Driver?> GetDriverByLicenseAsync(string license);
       
        Task<int> GetDriverTripCountAsync(string driverId);
        

   
        Task<IEnumerable<Trip>> GetDriverTripsAsync(string driverId);
        Task<IEnumerable<Trip>> GetDriverTripsAsyncByLicen(string license);
        Task<IEnumerable<Complaint>> GetDriverSubmitedComplainsBylicenAsync(string licen);
        Task<IEnumerable<Complaint>> GetDriverAgainstComplainsBylicenAsync(string licen);
        Task<Complaint> GetDriverComplainByComplainsIdAsync(Guid id);
        Task<double> GetDriverAvgRatingAsync(string licen);




        
        // Trips
        

        Task<Trip?> GetTripByIdAsync(Guid id);

        Task<IEnumerable<Trip>> GetTripsByStatusAsync(TripStatus status);

        
        

       


       
        // Dashboard / Reports
       
        Task<int> GetTotalTripsAsync();
        Task<int> GetTotalDriversAsync();
       

        
       

        Task SuspendAccountDriver(string lic);
        Task SuspendAccountRider(string id);
    }
}
