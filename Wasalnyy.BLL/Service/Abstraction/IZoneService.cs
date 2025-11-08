using Wasalnyy.BLL.DTO.Zone;

namespace Wasalnyy.BLL.Service.Abstraction
{
    public interface IZoneService
    {
        Task<ReturnZoneDto?> GetZoneAsync(decimal lat, decimal lng);
        Task CreateZoneAsync(CreateZoneDto dto);
        Task DeleteZoneAsync(Guid zoneId);
        Task UpdateZoneAsync(Guid zoneId, UpdateZoneDto dto );
    }
}
