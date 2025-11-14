using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wasalnyy.DAL.Database;
using Wasalnyy.DAL.Entities;
using Wasalnyy.DAL.Repo.Abstraction;

namespace Wasalnyy.DAL.Repo.Implementation
{
    public class ZoneRepo : IZoneRepo
    {
        private readonly WasalnyyDbContext _context;

        public ZoneRepo(WasalnyyDbContext context)
        {
            _context = context;
        }
        public async Task CreateAsync(Zone zone)
        {
            await _context.Zones.AddAsync(zone);
        }

        public async Task DeleteAsync(Guid id)
        {
            var zone = await _context.Zones.SingleOrDefaultAsync(x => x.Id == id);

            if (zone != null)
            {
                _context.Zones.Remove(zone);
            }
        }

        public async Task<IEnumerable<Zone>> GetCandidateZonesAsync(Coordinates coordinate)
        {
            return await _context.Zones.AsNoTracking()
                .Include(x=> x.Coordinates)
                .Where(z => coordinate.Lat >= z.MinLat 
                 && coordinate.Lat<= z.MaxLat
                 && coordinate.Lng >= z.MinLng 
                 && coordinate.Lng <= z.MaxLng)
                 .ToListAsync();
        }

        public async Task<Zone?> GetByIdAsync(Guid id)
        {
            return await _context.Zones.AsNoTracking().Include(x => x.Coordinates).SingleOrDefaultAsync(x => x.Id == id);
        }

        public async Task UpdateAsync(Zone zone)
        {
            _context.Entry(zone).State = EntityState.Modified;
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
