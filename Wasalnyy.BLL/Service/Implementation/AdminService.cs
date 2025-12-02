using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Wasalnyy.DAL.Repo.Abstraction;
using Wasalnyy.DAL.Repo.Implementation;

namespace Wasalnyy.BLL.Service.Implementation
{
    public class AdminService : IAdminService
    {
        private readonly IDriverRepo _driverRepo;
        private readonly IRiderRepo _riderRepo;
        private readonly ITripRepo _tripRepo;
        private readonly IComplaintRepo _complaintRepo;
        private readonly IReviewRepo _reviewRepo;

        public AdminService(IDriverRepo driverRepo, IRiderRepo riderRepo, ITripRepo tripRepo, IComplaintRepo complaintRepo, IReviewRepo reviewRepo)
        {
            _driverRepo = driverRepo;
            _riderRepo = riderRepo;
            _tripRepo = tripRepo;
            _complaintRepo = complaintRepo;
            _reviewRepo = reviewRepo;
        }

        
        // rider
        

        public async Task<IEnumerable<Rider>> GetAllRidersAsync()
        {
            return await _riderRepo.GetAllRidersAsync();
        }

        public async Task<Rider?> GetRiderByPhoneAsync(string phone)
        {
            return await _riderRepo.GetByPhoneAsync(phone);
        }

        

        

        public async Task<int> GetRiderTripCountAsync(string riderId)
        {
            return await _tripRepo.GetRiderTripsCountAsync(riderId);
        }

        public async Task<IEnumerable<Trip>> GetRiderTripsAsync(string riderId)
        {
            return await _tripRepo.GetAllRiderTripsPaginatedAsync(
                riderId,
                orderBy: trip => trip.RequestedDate,
                descending: true,
                pageNumber: 1,
                pageSize: 50
            );
        }


        
        //driver
        

        public async Task<IEnumerable<Driver>> GetAllDriversAsync()
        {
            return await _driverRepo.GetAllDriverAsync();
        }

        public async Task<Driver?> GetDriverByLicenseAsync(string licen)
        {
            return await _driverRepo.GetDriverByLicense(licen);
        }

      

       

        public async Task<int> GetDriverTripCountAsync(string driverId)
        {
            return await _tripRepo.GetDriverTripsCountAsync(driverId);
        }

        public async Task<IEnumerable<Trip>> GetDriverTripsAsync(string driverId)
        {
            return await _tripRepo.GetAllDriverTripsPaginatedAsync(
                driverId,
                orderBy: trip => trip.RequestedDate,
                descending: true,
                pageNumber: 1,
                pageSize: 50
            );
        }


        
        // trip
     

        public async Task<Trip?> GetTripByIdAsync(Guid id)
        {
            return await _tripRepo.GetByIdAsync(id);
        }

        public async Task<IEnumerable<Trip>> GetTripsByStatusAsync(TripStatus status)
        {
            return await _tripRepo.GetTripsByStatusAsync(status);
        }

        

       

       
        // Reports/stats
        

        public async Task<int> GetTotalDriversAsync()
        {
            return await _driverRepo.GetCountAsync();
        }

        

        public async Task<int> GetTotalTripsAsync()
        {
            return await _tripRepo.GetCountAsync();
        }

        

       

        public async Task SuspendAccountDriver(string lic)
        {
            var driver = await _driverRepo.GetDriverByLicense(lic);
            if (driver != null)
            {
                driver.Suspend(); 
                await _driverRepo.SaveChangesAsync();
            }
        }

        public async Task SuspendAccountRider(string id)
        {
            var driver = await _riderRepo.GetByIdAsync(id);
            if (driver != null)
            {
                driver.Suspend();
                await _riderRepo.SaveChangesAsync();
            }
        }

        public Task<double> GetRidersCount()
        {
            var count = _riderRepo.GetCountAsync();

            return count;
        }

        public async Task<IEnumerable<Trip>> GetRiderTripsAsyncByphone(string phonenum)
        {
            return await _riderRepo.GetRiderTripsByPhone(phonenum);
        }

        public async Task<IEnumerable<Complaint>> GetRiderComplainsByPhoneAsync(string phonenum)
        {
            return await _riderRepo.GetRiderComplainsByPhone(phonenum);
        }

     

        public Task<IEnumerable<Trip>> GetDriverTripsAsyncByLicen(string license)
        {
            //var driver=_driverRepo.GetDriverByLicense(license);
            //var trips = _tripRepo.GetAllDriverTripsPaginatedAsync(driver.Id);
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<Complaint>> GetDriverSubmitedComplainsBylicenAsync(string licen)
        {
            return await _complaintRepo.DriverComplains(licen);
        }

        public async Task<IEnumerable<Complaint>> GetDriverAgainstComplainsBylicenAsync(string licen)
        {
            return await _complaintRepo.DriverAgainstComplains(licen);
        }

        public async Task<double> GetDriverAvgRatingAsync(string licen)
        {
            var driver=await _driverRepo.GetDriverByLicense(licen);
            var rating = await _reviewRepo.GetDriverAverageRatingAsync(driver.Id);

            return rating;
        }

        public async Task<Complaint> GetRiderComplainByComplainsIdAsync(Guid id)
        {
            return await _complaintRepo.GetByIdAsync(id);
        }

        public async Task<Complaint> GetDriverComplainByComplainsIdAsync(Guid id)
        {
            return await _complaintRepo.GetByIdAsync(id);
        }

       
    }
}
