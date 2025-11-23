using AutoMapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wasalnyy.BLL.DTO;
using Wasalnyy.BLL.DTO.Driver;
using Wasalnyy.BLL.DTO.Rider;
using Wasalnyy.BLL.Enents;
using Wasalnyy.BLL.Exceptions;
using Wasalnyy.BLL.Service.Abstraction;
using Wasalnyy.BLL.Validators;
using Wasalnyy.DAL.Entities;
using Wasalnyy.DAL.Enum;
using Wasalnyy.DAL.Repo.Abstraction;
using Wasalnyy.DAL.Repo.Implementation;

namespace Wasalnyy.BLL.Service.Implementation
{
    public class DriverService : IDriverService
    {
        private readonly IDriverRepo _driverRepo;
        private readonly IZoneService _zoneService;
        private readonly DriverEvents _driverEvents;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly DriverServiceValidator _validator;
        private readonly IMapper _mapper;

        public DriverService(IDriverRepo driverRepo, DriverEvents driverEvents, IZoneService zoneService,
            IServiceScopeFactory serviceScopeFactory, DriverServiceValidator validator, IMapper mapper)
        {
            _driverRepo = driverRepo;
            _driverEvents = driverEvents;
            _zoneService = zoneService;
            _serviceScopeFactory = serviceScopeFactory;
            _validator = validator;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ReturnDriverDto>> GetAvailableDriversByZoneAsync(Guid zoneId)
        {
            _validator.ValidateGetAvailableDriversByZone(zoneId);
           return _mapper.Map< IEnumerable<Driver>, IEnumerable<ReturnDriverDto>> (await _driverRepo.GetAvailableDriversByZoneAsync(zoneId));
        }

        public async Task<ReturnDriverDto?> GetByIdAsync(string id)
        {
            _validator.ValidateGetById(id);
            var driver = await _driverRepo.GetByIdAsync(id);
            if(driver == null)
                throw new NotFoundException($"Driver with ID '{id}' was not found.");

            return _mapper.Map<Driver, ReturnDriverDto>(driver);
        }

        public async Task UpdateLocationAsync(string driverId, Coordinates coordinates)
        {
            _validator.ValidateUpdateLocation(driverId, coordinates);

            var driver = await _driverRepo.GetByIdAsync (driverId);

            if(driver == null)
                throw new NotFoundException($"Driver with ID '{driverId}' was not found.");

            var zone = await _zoneService.GetZoneAsync(coordinates);

            if (zone == null)
                _driverEvents.FireDriverOutOfZone(driverId);
            else
            {
                if (driver.ZoneId != zone.Id)
                    _driverEvents.FireDriverZoneChanged(driverId, driver.ZoneId, zone.Id);
                driver.ZoneId = zone.Id;
            }

            driver.Coordinates = coordinates;

            await _driverRepo.UpdateAsync(driver);
            await _driverRepo.SaveChangesAsync();

            _driverEvents.FireDriverLocationUpdated(driverId, coordinates);
        }

        public async Task SetDriverUnAvailableAsync(string driverId)
        {
            _validator.ValidateSetDriverOffline(driverId);

            var driver = await _driverRepo.GetByIdAsync(driverId);

            if (driver == null)
                throw new NotFoundException($"Driver with ID '{driverId}' was not found.");

            if(driver.DriverStatus == DriverStatus.UnAvailable)
                return;

            if (driver.DriverStatus != DriverStatus.Available)
                throw new AlreadyInTripException($"Driver with id {driverId} already in trip.");

            driver.DriverStatus = DriverStatus.UnAvailable;

            await _driverRepo.UpdateAsync(driver);
            await _driverRepo.SaveChangesAsync();

            _driverEvents.FireDriverStatusChangedToUnAvailable(driverId);
        }



        public async Task SetDriverInTripAsync(string driverId)
        {
            _validator.ValidateSetDriverInTrip(driverId);

            var driver = await _driverRepo.GetByIdAsync(driverId);

            if (driver == null)
                throw new NotFoundException($"Driver with ID '{driverId}' was not found.");

            if (driver.DriverStatus == DriverStatus.UnAvailable)
                throw new DriverIsOfflineException($"Driver with ID '{driverId}'is offline.");

            if (driver.DriverStatus != DriverStatus.Available)
                throw new AlreadyInTripException($"Driver with id {driverId} already in trip.");

            driver.DriverStatus = DriverStatus.InTrip;

            await _driverRepo.UpdateAsync(driver);
            await _driverRepo.SaveChangesAsync();
        }

        public async Task SetDriverAvailableAsync(string driverId, Coordinates coordinates)
        {
            _validator.ValidateSetDriverAvailable(driverId, coordinates);

            var driver = await _driverRepo.GetByIdAsync(driverId);

            if (driver == null)
                throw new NotFoundException($"Driver with ID '{driverId}' was not found.");

            if(driver.DriverStatus == DriverStatus.Available)
                throw new AlreadyAvailableException($"Driver with ID '{driverId}' already available.");

            var _tripService = _serviceScopeFactory.CreateScope().ServiceProvider.GetRequiredService<ITripService>();
            var activeTrip = await _tripService.GetDriverActiveTripAsync(driverId);

            if (activeTrip != null)
                throw new AlreadyInTripException($"Driver with id {driverId} already in trip.");

            var zone = await _zoneService.GetZoneAsync(coordinates);
            if (zone == null)
                throw new OutOfZoneException("You are out of zone.");

            driver.ZoneId = zone.Id;
            driver.DriverStatus = DriverStatus.Available;
            driver.Coordinates = coordinates;

            await _driverRepo.UpdateAsync(driver);
            await _driverRepo.SaveChangesAsync();

            _driverEvents.FireDriverStatusChangedToAvailable(driverId, zone.Id);
        }
        public async Task<bool> UpdateDriverInfo(string id, DriverUpdateDto driverUpdate)
        {
            var oldriderinfos = await _driverRepo.GetByIdAsync(id);
            if (oldriderinfos == null)
            {
                return false;
            }

            _mapper.Map(driverUpdate, oldriderinfos);
            await _driverRepo.UpdateAsync(oldriderinfos);
            await _driverRepo.SaveChangesAsync();
            return true;

        }

        public async Task<string> DriverNameAsync(string driverId)
        {
            var driver = await _driverRepo.GetByIdAsync(driverId) ;
            if (driver == null)
                return string.Empty;

            return driver.FullName;
        }

        public async Task<string?> DriverProfileImageAsync(string driverId)
        {
            var driver = await _driverRepo.GetByIdAsync(driverId);
            if (driver == null)
                return string.Empty;

            return driver.Image;
        }

        public async Task<DriverStatus> GetDriverStatusAsync(string driverId)
        {
            var driver = await _driverRepo.GetByIdAsync(driverId);
            return driver.DriverStatus;

        }
            

        public async Task<int> GetTotalCompletedTripsAsync(string driverId)
        {
            var driver = await _driverRepo.GetByIdAsync(driverId);
            if (driver == null)
                return 0;

            return driver.Trips.Count(t => t.TripStatus == TripStatus.Ended);
        }

        public Task<decimal> GetDriverRatingAsync(string driverId)
        {
            throw new NotImplementedException();
        }

        public async Task<VehicleDto?> GetDriverVehicleInfoAsync(string driverId)
        {
            var driver = await _driverRepo.GetByIdAsync(driverId);
            if(driver == null)
            {
                return null;
            }
            return _mapper.Map<VehicleDto>(driver.Vehicle);

        }

        public async Task<bool> IsDriverSuspendedAsync(string driverId)
        {

            var driver = await _driverRepo.GetByIdAsync(driverId);
            if (driver == null)
                return false;

            return driver.IsSuspended;
        }

        public Task<decimal> GetDriverWalletBalanceAsync(string driverId)
        {
            throw new NotImplementedException();
        }
    }
}
