using Wasalnyy.BLL.DTO.Zone;
using Wasalnyy.DAL.Entities;

namespace Wasalnyy.BLL.Service.Abstraction
{
    public interface IZoneService
    {
        Task<ReturnZoneDto?> GetZoneAsync(Guid zoneId);
        Task<ReturnZoneDto?> GetZoneAsync(Coordinates coordinate);
        Task CreateZoneAsync(CreateZoneDto dto);
        Task DeleteZoneAsync(Guid zoneId);
        Task UpdateZoneAsync(Guid zoneId, UpdateZoneDto dto);
    }
}
