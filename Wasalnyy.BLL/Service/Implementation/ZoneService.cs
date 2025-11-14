using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wasalnyy.BLL.DTO.Zone;
using Wasalnyy.BLL.Service.Abstraction;
using Wasalnyy.DAL.Entities;

namespace Wasalnyy.BLL.Service.Implementation
{
    public class ZoneService : IZoneService
    {
        public Task CreateZoneAsync(CreateZoneDto dto)
        {
            throw new NotImplementedException();
        }

        public Task DeleteZoneAsync(Guid zoneId)
        {
            throw new NotImplementedException();
        }

        public Task<ReturnZoneDto?> GetZoneAsync(Guid zoneId)
        {
            throw new NotImplementedException();
        }

        public Task<ReturnZoneDto?> GetZoneAsync(Coordinates coordinate)
        {
            throw new NotImplementedException();
        }

        public Task UpdateZoneAsync(Guid zoneId, UpdateZoneDto dto)
        {
            throw new NotImplementedException();
        }
    }
}
