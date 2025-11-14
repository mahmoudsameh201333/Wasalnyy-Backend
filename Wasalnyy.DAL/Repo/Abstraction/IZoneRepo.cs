using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wasalnyy.DAL.Entities;

namespace Wasalnyy.DAL.Repo.Abstraction
{
    public interface IZoneRepo
    {
        Task<Zone?> GetByIdAsync(Guid id);
        Task<IEnumerable<Zone>> GetCandidateZonesAsync(Coordinates coordinate);
        Task CreateAsync(Zone zone);
        Task DeleteAsync(Guid id);
        Task UpdateAsync(Zone zone);
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    }
}
