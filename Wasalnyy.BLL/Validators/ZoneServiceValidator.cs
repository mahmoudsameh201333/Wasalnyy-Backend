namespace Wasalnyy.BLL.Validators
{
    public class ZoneServiceValidator
    {
        public async Task ValidateCreateZone(CreateZoneDto dto)
        {
            ArgumentNullException.ThrowIfNull(dto);

            if(dto.Coordinates.Count() == 0)
                throw new ArgumentException();
        }

        public async Task ValidateDeleteZone(Guid zoneId)
        {
            if (zoneId == Guid.Empty)
                throw new ArgumentException($"zoneId '{zoneId}' is empty");
        }

        public async Task ValidateGetById(Guid zoneId)
        {
            if (zoneId == Guid.Empty)
                throw new ArgumentException($"zoneId '{zoneId}' is empty");
        }

        public async Task ValidateGetZone(Coordinates coordinates)
        {
            ArgumentNullException.ThrowIfNull(coordinates);
        }

        public async Task ValidateUpdateZone(Guid zoneId, UpdateZoneDto dto)
        {
            if (zoneId == Guid.Empty)
                throw new ArgumentException($"zoneId '{zoneId}' is empty");

            ArgumentNullException.ThrowIfNull(dto);

            if (dto.Coordinates.Count() == 0)
                throw new ArgumentException();
        }
    }
}
