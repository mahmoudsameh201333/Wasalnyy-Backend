using AutoMapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wasalnyy.BLL.DTO.Driver;
using Wasalnyy.BLL.Enents;
using Wasalnyy.BLL.Exceptions;
using Wasalnyy.BLL.Service.Abstraction;
using Wasalnyy.DAL.Entities;
using Wasalnyy.DAL.Enum;
using Wasalnyy.DAL.Repo.Abstraction;

namespace Wasalnyy.BLL.Service.Implementation
{
    public class DriverService : IDriverService
    {
        private readonly IDriverRepo _driverRepo;
        private readonly IZoneService _zoneService;
        private readonly DriverEvents _driverEvents;
        private readonly IMapper _mapper;

        public DriverService(IDriverRepo driverRepo, DriverEvents driverEvents, IZoneService zoneService, IMapper mapper)
        {
            _driverRepo = driverRepo;
            _driverEvents = driverEvents;
            _zoneService = zoneService;
            _mapper = mapper;
        }

        private async Task ChangeStatusAsync(string driverId, DriverStatus status)
        {
            await _driverRepo.ChangeStatusAsync(driverId, status);
            await _driverRepo.SaveChangesAsync();
        }

        public async Task<IEnumerable<ReturnDriverDto>> GetAvailableDriversByZoneAsync(Guid zoneId)
        {
           return _mapper.Map< IEnumerable<Driver>, IEnumerable<ReturnDriverDto>> (await _driverRepo.GetAvailableDriversByZoneAsync(zoneId));
        }

        public async Task<ReturnDriverDto?> GetByIdAsync(string id)
        {
            var driver = await _driverRepo.GetByIdAsync(id);
            if(driver == null)
                throw new NotFoundException($"Driver with ID '{id}' was not found.");

            return _mapper.Map<Driver, ReturnDriverDto>(driver);
        }

        public async Task UpdateLocationAsync(string driverId, Coordinates coordinate)
        {
            var driver = await _driverRepo.GetByIdAsync (driverId);

            if(driver == null)
                throw new NotFoundException($"Driver with ID '{driverId}' was not found.");

            _driverEvents.FireDriverLocationUpdated(driverId, coordinate.Lng, coordinate.Lat);

            var zone = await _zoneService.GetZoneAsync(coordinate);
            
            if (zone == null)
                throw new OutOfZoneException("You are out of zone.");

            if (driver.ZoneId != zone.Id)
                _driverEvents.FireDriverZoneChanged(driverId, zone.Id);

            await _driverRepo.UpdateDriverZoneAsync(driverId, zone.Id);
            await _driverRepo.SaveChangesAsync();
        }

        public async Task SetDriverOfflineAsync(string driverId)
        {
            await ChangeStatusAsync(driverId, DriverStatus.Offline);
        }



        public async Task SetDriverInTripAsync(string driverId, Guid tripId)
        {
            await ChangeStatusAsync(driverId, DriverStatus.InTrip);
            _driverEvents.FireDriverStatusChangedToInTrip(driverId, tripId);
        }

        public async Task SetDriverAvailableAsync(string driverId)
        {
            await ChangeStatusAsync(driverId, DriverStatus.Available);
            _driverEvents.FireDriverStatusChangedToAvailable(driverId);
        }
    }
}
