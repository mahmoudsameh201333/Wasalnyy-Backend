using Microsoft.EntityFrameworkCore;
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
    }
}
