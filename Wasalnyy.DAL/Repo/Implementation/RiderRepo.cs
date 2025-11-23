using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wasalnyy.DAL.Database;
using Wasalnyy.DAL.Repo.Abstraction;

namespace Wasalnyy.DAL.Repo.Implementation
{
    public class RiderRepo : IRiderRepo
    {
        private readonly WasalnyyDbContext _context;

        public RiderRepo(WasalnyyDbContext context)
        {
            _context = context;
        }
        public async Task<Rider?> GetByIdAsync(string driverId)
        {
            return await _context.Riders.AsNoTracking()
                .SingleOrDefaultAsync(x => x.Id == driverId);
        }
        public async Task<IEnumerable<Rider>> GetAllRidersAsync()
        {
            return await _context.Riders
                .AsNoTracking()
                .ToListAsync();
        }

        public Task<Rider?> GetByPhoneAsync(string phonenum)
        {
            var RiderByPhone=_context.Riders.FirstOrDefaultAsync(e=>e.PhoneNumber == phonenum);
            return RiderByPhone;
        }
        public async Task<int> GetCountAsync()
        {
            return await _context.Riders
                .AsNoTracking()
                .CountAsync(r=>r.IsDeleted==false);
        }

        public async Task UpdateRiderAsync(Rider rider)
        {
            _context.Entry(rider).State = EntityState.Modified;

        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }

    }
}
