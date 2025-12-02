namespace Wasalnyy.BLL.Service.Implementation
{
    public class ZoneService : IZoneService
    {
        private readonly IZoneRepo _zoneRepo;
        private readonly ZoneServiceValidator _validator;
        private readonly IMapper _mapper;

        public ZoneService(IZoneRepo zoneRepo, ZoneServiceValidator validator, IMapper mapper) 
        {
            _zoneRepo = zoneRepo;
            _validator = validator;
            _mapper = mapper;
        }
        public async Task CreateZoneAsync(CreateZoneDto dto)
        {
            await _validator.ValidateCreateZone(dto);

            var zone = _mapper.Map<CreateZoneDto, Zone>(dto);

            (zone.MinLat, zone.MaxLat, zone.MinLng, zone.MaxLng) = ZoneHelper.GetBoundingBoxAsync(zone.Coordinates.ToList());

            await _zoneRepo.CreateAsync(zone);
            await _zoneRepo.SaveChangesAsync();
        }

        public async Task DeleteZoneAsync(Guid zoneId)
        {
            await _validator.ValidateDeleteZone(zoneId);

            await _zoneRepo.DeleteAsync(zoneId);
            await _zoneRepo.SaveChangesAsync();
        }

        public async Task<IEnumerable<ReturnZoneDto>> GetAllAsync()
        {
            return _mapper.Map<IEnumerable<Zone>, IEnumerable<ReturnZoneDto>>(await _zoneRepo.GetAllAsync());
        }

        public async Task<ReturnZoneDto?> GetByIdAsync(Guid zoneId)
        {
            await _validator.ValidateGetById(zoneId);

            return _mapper.Map<Zone?, ReturnZoneDto?>(await _zoneRepo.GetByIdAsync(zoneId));
        }

        public async Task<ReturnZoneDto?> GetZoneAsync(Coordinates coordinate)
        {
            await _validator.ValidateGetZone(coordinate);

            var candidateZones = await _zoneRepo.GetCandidateZonesAsync(coordinate);

            foreach (var zone in candidateZones)
            {
                if (ZoneHelper.IsPointInPolygon(coordinate.Lat, coordinate.Lng, zone.Coordinates.ToList()))
                    return _mapper.Map<Zone, ReturnZoneDto>(zone);
            }
            return null;
        }

        public async Task UpdateZoneAsync(Guid zoneId, UpdateZoneDto dto)
        {
            await _validator.ValidateUpdateZone(zoneId, dto);

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
