using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Wasalnyy.DAL.Repo.Abstraction;

namespace Wasalnyy.BLL.Service.Implementation
{
    public class AdminService : IAdminService
    {
        private readonly IDriverRepo _driverRepo;
        private readonly IRiderRepo _riderRepo;
        private readonly ITripRepo _tripRepo;

        public AdminService(IDriverRepo driverRepo, IRiderRepo riderRepo, ITripRepo tripRepo)
        {
            _driverRepo = driverRepo;
            _riderRepo = riderRepo;
            _tripRepo = tripRepo;
        }

        // ============================================================
        // 🟦 R I D E R   M A N A G E M E N T
        // ============================================================

        public async Task<IEnumerable<Rider>> GetAllRidersAsync()
        {
            return await _riderRepo.GetAllRidersAsync();
        }

        public async Task<Rider?> GetRiderByPhoneAsync(string phone)
        {
            return await _riderRepo.GetByPhoneAsync(phone);
        }

        public Task<bool> UpdateRiderAsync(string id, UpdateRider rider)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteRiderAsync(string riderId)
        {
            throw new NotImplementedException();
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


        // ============================================================
        // 🟧 D R I V E R   M A N A G E M E N T
        // ============================================================

        public async Task<IEnumerable<Driver>> GetAllDriversAsync()
        {
            return await _driverRepo.GetAllDriverAsync();
        }

        public async Task<Driver?> GetDriverByLicenseAsync(string licen)
        {
            return await _driverRepo.GetDriverByLicense(licen);
        }

        public Task<bool> UpdateDriverAsync(string id, UpdateDriver driver)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteUserAsync(string userId)
        {
            throw new NotImplementedException();
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


        // ============================================================
        // T R I P   M A N A G E M E N T
        // ============================================================

        public async Task<Trip?> GetTripByIdAsync(Guid id)
        {
            return await _tripRepo.GetByIdAsync(id);
        }

        public async Task<IEnumerable<Trip>> GetTripsByStatusAsync(TripStatus status)
        {
            return await _tripRepo.GetTripsByStatusAsync(status);
        }

        

       

       

        // ============================================================
        // 🟪 R E P O R T S /  S T A T I S T I C S
        // ============================================================

        public async Task<int> GetTotalDriversAsync()
        {
            return await _driverRepo.GetCountAsync();
        }

        public async Task<int> GetTotalRidersAsync()
        {
            return await _riderRepo.GetCountAsync();
        }

        public async Task<int> GetTotalTripsAsync()
        {
            return await _tripRepo.GetCountAsync();
        }

        

        public Task<decimal> GetRevenueAsync(DateTime from, DateTime to)
        {
            throw new NotImplementedException();
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
    }
}
