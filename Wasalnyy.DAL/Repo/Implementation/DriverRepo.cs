using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wasalnyy.DAL.Database;
using Wasalnyy.DAL.Entities;
using Wasalnyy.DAL.Enum;
using Wasalnyy.DAL.Repo.Abstraction;

namespace Wasalnyy.DAL.Repo.Implementation
{
    public class DriverRepo : IDriverRepo
    {
        private readonly WasalnyyDbContext _context;

        public DriverRepo(WasalnyyDbContext context)
        {
            _context = context;
        }

        public async Task UpdateAsync(Driver driver)
        {
            _context.Entry(driver).State = EntityState.Modified;
        }

        public async Task<IEnumerable<Driver>> GetAvailableDriversByZoneAsync(Guid zoneId)
        {
            return await _context.Drivers.AsNoTracking()
                .Include(d => d.Vehicle)
                .Include(d => d.Zone)
                .Where(x => x.ZoneId == zoneId && x.DriverStatus == DriverStatus.Available)
                .ToListAsync();
        }

        public async Task<Driver?> GetByIdAsync(string driverId)
        {
            return await _context.Drivers.AsNoTracking()
                .Include(d => d.Vehicle)
                .Include(d => d.Zone)
                .SingleOrDefaultAsync(x => x.Id == driverId);
        }


        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task ChangeStatusAsync(string driverId, DriverStatus status)
        {
            var driver = await _context.Drivers.SingleOrDefaultAsync(x => x.Id == driverId);

            if (driver != null)
            {
                driver.DriverStatus = status;
                _context.Entry(driver).State = EntityState.Modified;
            }
        }

        public async Task UpdateDriverZoneAsync(string driverId, Guid zoneId)
        {
            var driver = await _context.Drivers.SingleOrDefaultAsync(x => x.Id == driverId);

            if (driver != null)
            {
                driver.ZoneId = zoneId;
                _context.Entry(driver).State = EntityState.Modified;
            }
        }

        public async Task<IEnumerable<Driver>> GetAllDriverAsync()
        {
            return await _context.Drivers
                .AsNoTracking()
                .ToListAsync();
        }
        public async Task<Driver> GetDriverByLicense(string licen)
        {

            var Driver = await _context.Drivers.SingleOrDefaultAsync(e => e.License == licen);
            if (Driver != null)
            {
                return Driver;
            }
            else return null;
        }
        public async Task<int> GetCountAsync()
        {
            return await _context.Drivers
                .AsNoTracking()
                .CountAsync(d=>d.IsDeleted==false);
        }


    }
}
