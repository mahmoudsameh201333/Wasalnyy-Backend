using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Wasalnyy.DAL.Database;
using Wasalnyy.DAL.Entities;
using Wasalnyy.DAL.Enum;
using Wasalnyy.DAL.Repo.Abstraction;

namespace Wasalnyy.DAL.Repo.Implementation
{
    public class TripRepo : ITripRepo
    {
        private readonly WasalnyyDbContext _context;

        public TripRepo(WasalnyyDbContext context)
        {
            _context = context;
        }

        public async Task CreateTripAsync(Trip trip)
        {
            await _context.Trips.AddAsync(trip);
        }

        public async Task DeleteAsync(Guid id)
        {
            var trip = await _context.Trips.SingleOrDefaultAsync(x => x.Id == id);

            if (trip != null)
                _context.Trips.Remove(trip);
            
        }

        public async Task<IEnumerable<Trip>> GetTripsPaginatedAsync(Expression<Func<Trip, object>> orderBy, bool descending = false, int pageNumber = 1, int pageSize = 10)
        {
            var query = _context.Trips.AsNoTracking()
                .Include(x => x.Driver)
                .Include(x => x.Rider)
                .Include(x => x.Zone)
                .AsQueryable();

            query = descending? query.OrderByDescending(orderBy) : query.OrderBy(orderBy);

            return  await query.Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(); 
        }

        public async Task<IEnumerable<Trip>> GetAllDriverTripsPaginatedAsync(string driverId, Expression<Func<Trip, object>> orderBy, bool descending = false, int pageNumber = 1, int pageSize = 10)
        {
            var query = _context.Trips.AsNoTracking()
                .Include(x => x.Driver)
                .Include(x => x.Rider)
                .Include(x => x.Zone)
                .Where(x=> x.DriverId == driverId);

            query = descending ? query.OrderByDescending(orderBy) : query.OrderBy(orderBy);

            return await query.Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<IEnumerable<Trip>> GetAllRiderTripsPaginatedAsync(string riderId, Expression<Func<Trip, object>> orderBy, bool descending = false, int pageNumber = 1, int pageSize = 10)
        {
            var query = _context.Trips.AsNoTracking()
                .Include(x => x.Driver)
                .Include(x => x.Rider)
                .Include(x => x.Zone)
                .Where(x => x.RiderId == riderId);

            query = descending == true ? query.OrderByDescending(orderBy) : query.OrderBy(orderBy);

            return await query.Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<Trip?> GetByIdAsync(Guid id)
        {
            return await _context.Trips.AsNoTracking()
                .Include(x => x.Driver)
                .Include(x => x.Rider)
                .Include(x => x.Zone)
                .SingleOrDefaultAsync(x => x.Id == id);
        }

        public async Task<IEnumerable<Trip>> GetRequestedTripsByZoneAsync(Guid zoneId)
        {
            return await _context.Trips.AsNoTracking()
                .Include(x => x.Driver)
                .Include(x => x.Rider)
                .Include(x => x.Zone)
                .Where(x=> x.ZoneId == zoneId && x.TripStatus == TripStatus.Requested)
                .ToListAsync();
        }

        public async Task UpdateTripAsync(Trip trip)
        {
            _context.Entry(trip).State = EntityState.Modified;
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task<int> GetCountAsync()
        {
            return await _context.Trips.AsNoTracking().CountAsync();
        }

        public async Task<int> GetRiderTripsCountAsync(string riderId)
        {
            return await _context.Trips.AsNoTracking().CountAsync(x=> x.RiderId == riderId);
        }

        public async Task<int> GetDriverTripsCountAsync(string driverId)
        {
            return await _context.Trips.AsNoTracking().CountAsync(x => x.DriverId == driverId);
        }

        public async Task<IEnumerable<Trip>> GetTripsByStatusAsync(TripStatus status)
        {
            return await _context.Trips
                .AsNoTracking() 
                .Where(t => t.TripStatus == status)
                .Include(t => t.Driver)
                .Include(t => t.Rider)
                .ToListAsync();
        }

        public Task<double> GetTripDurationAsync(Guid id)
        {
            throw new NotImplementedException();
        }
    }
}
