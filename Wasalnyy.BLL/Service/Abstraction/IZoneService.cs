using Wasalnyy.BLL.DTO.Zone;
using Wasalnyy.DAL.Entities;

namespace Wasalnyy.BLL.Service.Abstraction
{
    public interface IZoneService
    {
        Task<ReturnZoneDto?> GetByIdAsync(Guid zoneId);
        Task<IEnumerable<ReturnZoneDto>> GetAllAsync();
        Task<ReturnZoneDto?> GetZoneAsync(Coordinates coordinate);
        Task CreateZoneAsync(CreateZoneDto dto);
        Task DeleteZoneAsync(Guid zoneId);
        Task UpdateZoneAsync(Guid zoneId, UpdateZoneDto dto);
    }
}
