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
            await _validator.ValidateGetAvailableDriversByZone(zoneId);
           return _mapper.Map< IEnumerable<Driver>, IEnumerable<ReturnDriverDto>> (await _driverRepo.GetAvailableDriversByZoneAsync(zoneId));
        }

        public async Task<ReturnDriverDto?> GetByIdAsync(string id)
        {
            await _validator.ValidateGetById(id);
            var driver = await _driverRepo.GetByIdAsync(id);

            Console.WriteLine($" Service: Driver found: {driver != null}");  

            if (driver == null)
                throw new NotFoundException($"Driver with ID '{id}' was not found.");

            Console.WriteLine($" Service: Driver name: {driver.FullName}");  

            var result = _mapper.Map<Driver, ReturnDriverDto>(driver);

            Console.WriteLine($" Service: Mapped successfully");  

            return result;
        }

        public async Task UpdateLocationAsync(string driverId, Coordinates coordinates)
        {
            await _validator.ValidateUpdateLocation(driverId, coordinates);

            var driver = await _driverRepo.GetByIdAsync (driverId);

            if(driver == null)
                throw new NotFoundException($"Driver with ID '{driverId}' was not found.");

            var zone = await _zoneService.GetZoneAsync(coordinates);

            if (zone == null)
                await _driverEvents.FireDriverOutOfZone(driverId);
            else
            {
                if (driver.ZoneId != zone.Id)
                    await _driverEvents.FireDriverZoneChanged(driverId, driver.ZoneId, zone.Id);
                driver.ZoneId = zone.Id;
            }

            driver.Coordinates = coordinates;

            await _driverRepo.UpdateAsync(driver);
            await _driverRepo.SaveChangesAsync();

            await _driverEvents.FireDriverLocationUpdated(driverId, coordinates);
        }

        public async Task SetDriverUnAvailableAsync(string driverId)
        {
            await _validator.ValidateSetDriverOffline(driverId);

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

            await _driverEvents.FireDriverStatusChangedToUnAvailable(driverId);
        }



        public async Task SetDriverInTripAsync(string driverId)
        {
            await _validator.ValidateSetDriverInTrip(driverId);

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
            await _validator.ValidateSetDriverAvailable(driverId, coordinates);

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

            await _driverEvents.FireDriverStatusChangedToAvailable(driverId, zone.Id);
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
