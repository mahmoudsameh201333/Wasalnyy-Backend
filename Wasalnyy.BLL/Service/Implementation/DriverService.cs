using AutoMapper;
using System;
using System.Collections.Generic;
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


        public async Task ChangeStatusAsync(string driverId, DriverStatus status)
        {
            await _driverRepo.ChangeStatusAsync(driverId, status);
            await _driverRepo.SaveChangesAsync();
            _driverEvents.FireDriverStatusChanged(driverId, status);
        }

        public async Task<IEnumerable<ReturnDriver>> GetAvailableDriversByZoneAsync(Guid zoneId)
        {
           return _mapper.Map< IEnumerable<Driver>, IEnumerable<ReturnDriver>> (await _driverRepo.GetAvailableDriversByZoneAsync(zoneId));
        }

        public async Task<ReturnDriver?> GetByIdAsync(string id)
        {
            var driver = await _driverRepo.GetByIdAsync(id);
            if(driver == null)
                throw new NotFoundException($"Driver with ID '{id}' was not found.");

            return _mapper.Map<Driver, ReturnDriver>(driver);
        }

        public async Task UpdateLocationAsync(string driverId, Coordinate coordinate)
        {
            var zone = await _zoneService.GetZoneAsync(coordinate);
            
            if (zone == null)
                throw new OutOfZoneException("You are out of zone.");

            await _driverRepo.UpdateDriverZoneAsync(driverId, zone.Id);
            await _driverRepo.SaveChangesAsync();
        }
    }
}
