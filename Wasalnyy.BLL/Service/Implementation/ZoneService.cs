using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Wasalnyy.BLL.DTO.Zone;
using Wasalnyy.BLL.Exceptions;
using Wasalnyy.BLL.Helper;
using Wasalnyy.BLL.Service.Abstraction;
using Wasalnyy.DAL.Entities;
using Wasalnyy.DAL.Repo.Abstraction;

namespace Wasalnyy.BLL.Service.Implementation
{
    public class ZoneService : IZoneService
    {
        private readonly IZoneRepo _zoneRepo;
        private readonly IMapper _mapper;

        public ZoneService(IZoneRepo zoneRepo, IMapper mapper) 
        {
            _zoneRepo = zoneRepo;
            _mapper = mapper;
        }
        public async Task CreateZoneAsync(CreateZoneDto dto)
        {
            var zone = _mapper.Map<CreateZoneDto, Zone>(dto);

            (zone.MinLat, zone.MaxLat, zone.MinLng, zone.MaxLng) = ZoneHelper.GetBoundingBoxAsync(zone.Coordinates.ToList());

            await _zoneRepo.CreateAsync(zone);
            await _zoneRepo.SaveChangesAsync();
        }

        public async Task DeleteZoneAsync(Guid zoneId)
        {
            await _zoneRepo.DeleteAsync(zoneId);
            await _zoneRepo.SaveChangesAsync();
        }

        public async Task<IEnumerable<ReturnZoneDto>> GetAllAsync()
        {
            return _mapper.Map<IEnumerable<Zone>, IEnumerable<ReturnZoneDto>>(await _zoneRepo.GetAllAsync());
        }

        public async Task<ReturnZoneDto?> GetByIdAsync(Guid zoneId)
        {
            return _mapper.Map<Zone?, ReturnZoneDto?>(await _zoneRepo.GetByIdAsync(zoneId));
        }

        public async Task<ReturnZoneDto?> GetZoneAsync(Coordinates coordinate)
        {
            var candidateZones = await _zoneRepo.GetCandidateZonesAsync(coordinate);

            foreach (var zone in candidateZones)
            {
                if (ZoneHelper.IsPointInPolygon(coordinate.Lat, coordinate.Lng, zone.Coordinates.ToList()))
                    return _mapper.Map<Zone, ReturnZoneDto>(zone);
            }
            return null;
            //throw new OutOfZoneException($"Coordinates '{coordinate.Lat} ,{coordinate.Lng}' out of zone.");
        }

        public async Task UpdateZoneAsync(Guid zoneId, UpdateZoneDto dto)
        {
            var zone = await _zoneRepo.GetByIdAsync(zoneId);

            if (zone == null)
                throw new NotFoundException($"Zone with ID '{zoneId}' was not found.");

            var updatedZone = _mapper.Map(dto, zone);
            updatedZone.Id = zoneId;

            await _zoneRepo.UpdateAsync(updatedZone);
            await  _zoneRepo.SaveChangesAsync();
        }
    }
}
