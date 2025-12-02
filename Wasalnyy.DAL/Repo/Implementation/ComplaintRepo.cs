using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wasalnyy.DAL.Database;
using Wasalnyy.DAL.Entities;
using Wasalnyy.DAL.Repo.Abstraction;

namespace Wasalnyy.DAL.Repo.Implementation
{
    public class ComplaintRepo : IComplaintRepo
    {
        private readonly WasalnyyDbContext _db;

        public ComplaintRepo(WasalnyyDbContext db)
        {
            _db = db ;
        }

       
        public async Task AddAsync(Complaint complaint)
        {
            if (complaint == null)
                throw new ArgumentNullException(nameof(complaint), "Complaint cannot be null");

            await _db.Complaints.AddAsync(complaint);
        }

       
        public async Task<Complaint> GetByIdAsync(Guid id)
        {
            if (id == Guid.Empty)
                throw new ArgumentNullException(nameof(id), "Complaint ID cannot be empty");

            return await _db.Complaints
                .Include(c => c.Trip)
                .Include(c => c.SubmittedBy)
                .Include(c => c.AgainstUser)
                .FirstOrDefaultAsync(x => x.Id == id);
        }


        public async Task<IEnumerable<Complaint>> GetAllComplaintsAsync()
        {
            return await _db.Complaints
                .Include(c => c.Trip)
                .Include(c => c.SubmittedBy)
                .Include(c => c.AgainstUser)
                .OrderByDescending(c => c.CreatedAt)
                .AsNoTracking()
                .ToListAsync();
        }

      
        public async Task<IEnumerable<Complaint>> GetComplaintsAgainstUserAsync(string userId)
        {
            if (string.IsNullOrEmpty(userId))
                throw new ArgumentNullException(nameof(userId), "User ID cannot be null or empty");

            return await _db.Complaints
                .Where(c => c.AgainstUserId == userId)
                .Include(c => c.Trip)
                .Include(c => c.SubmittedBy)
                .OrderByDescending(c => c.CreatedAt)
                .AsNoTracking()
                .ToListAsync();
        }

       
        public async Task<IEnumerable<Complaint>> GetComplaintsByUserAsync(string userId)
        {
            if (string.IsNullOrEmpty(userId))
                throw new ArgumentNullException(nameof(userId), "User ID cannot be null or empty");

            return await _db.Complaints
                .Where(c => c.SubmittedById == userId)
                .Include(c => c.Trip)
                .Include(c => c.AgainstUser)
                .OrderByDescending(c => c.CreatedAt)
                .AsNoTracking()
                .ToListAsync();
        }

       
        public async Task<IEnumerable<Complaint>> GetCriticalComplaintsAgainstUserAsync(string userId)
        {
            if (string.IsNullOrEmpty(userId))
                throw new ArgumentNullException(nameof(userId), "User ID cannot be null or empty");

            var criticalCategories = new[]
            {
                ComplaintCategory.Harassment,
                ComplaintCategory.DangerousDriving,
                ComplaintCategory.Theft,
                ComplaintCategory.IncorrectCharges,
                ComplaintCategory.PhysicalConflict
            };

            return await _db.Complaints
                .Where(c => c.AgainstUserId == userId && criticalCategories.Contains(c.Category))
                .OrderByDescending(c => c.CreatedAt)
                .AsNoTracking()
                .ToListAsync();
        }

        
        public async Task<IEnumerable<Complaint>> GetNonCriticalComplaintsAgainstUserAsync(string userId)
        {
            if (string.IsNullOrEmpty(userId))
                throw new ArgumentNullException(nameof(userId), "User ID cannot be null or empty");

            var nonCriticalCategories = new[]
            {
                ComplaintCategory.DriverRude,
                ComplaintCategory.CarIssue,
                ComplaintCategory.LateArrival,
                ComplaintCategory.RiderRude,
                ComplaintCategory.WrongCancellation,
                ComplaintCategory.Other
            };

            return await _db.Complaints
                .Where(c => c.AgainstUserId == userId && nonCriticalCategories.Contains(c.Category))
                .OrderByDescending(c => c.CreatedAt)
                .AsNoTracking()
                .ToListAsync();
        }

     
        public async Task<UserComplaintStatistics> GetUserComplaintStatisticsAsync(string userId)
        {
            if (string.IsNullOrEmpty(userId))
                throw new ArgumentNullException(nameof(userId), "User ID cannot be null or empty");

            var criticalCategories = new[]
            {
                ComplaintCategory.Harassment,
                ComplaintCategory.DangerousDriving,
                ComplaintCategory.Theft,
                ComplaintCategory.IncorrectCharges,
                ComplaintCategory.PhysicalConflict
            };

            var nonCriticalCategories = new[]
            {
                ComplaintCategory.DriverRude,
                ComplaintCategory.CarIssue,
                ComplaintCategory.LateArrival,
                ComplaintCategory.RiderRude,
                ComplaintCategory.WrongCancellation,
                ComplaintCategory.Other
            };

            var complaints = await _db.Complaints
                .Where(c => c.AgainstUserId == userId && c.Status != ComplaintStatus.Dismissed)
                .ToListAsync();

            var criticalCount = complaints.Count(c => criticalCategories.Contains(c.Category));
            var nonCriticalCount = complaints.Count(c => nonCriticalCategories.Contains(c.Category));

            return new UserComplaintStatistics
            {
                UserId = userId,
                CriticalComplaintsCount = criticalCount,
                NonCriticalComplaintsCount = nonCriticalCount,
                
            };
        }

        
        
       
        public async Task<bool> HasComplainedAboutTripAsync(Guid tripId, string userId)
        {
            if (tripId == Guid.Empty)
                throw new ArgumentNullException(nameof(tripId), "Trip ID cannot be empty");

            if (string.IsNullOrEmpty(userId))
                throw new ArgumentNullException(nameof(userId), "User ID cannot be null or empty");

            return await _db.Complaints
                .AnyAsync(c => c.TripId == tripId && c.SubmittedById == userId);
        }

        
        public async Task UpdateComplaintStatusAsync(Guid complaintId, ComplaintStatus status)
        {
            if (complaintId == Guid.Empty)
                throw new ArgumentNullException( "Complaint ID cannot be empty");

            var complaint = await _db.Complaints.FindAsync(complaintId);
            if (complaint == null)
                throw new InvalidOperationException("Complaint not found");

            complaint.Status = status;
            complaint.ResolvedAt = DateTime.UtcNow;

            _db.Complaints.Update(complaint);
        }
        public async Task<IEnumerable<Complaint>>DriverComplains(string licen)
        {
            var driver = await _db.Drivers.FirstOrDefaultAsync(d => d.License == licen);
            if (driver == null)
            {
                throw new ArgumentNullException("Drivr can't be found");
            }
            var complains = await _db.Complaints.Where(d => d.SubmittedById == driver.Id).AsNoTracking().ToListAsync();
            return complains;
        }

        public async Task<IEnumerable<Complaint>> DriverAgainstComplains(string licen)
        {
            var driver = await _db.Drivers.FirstOrDefaultAsync(d => d.License == licen);
            if (driver == null)
            {
                throw new ArgumentNullException("Drivr can't be found");
            }
            var complains = await _db.Complaints.Where(d => d.AgainstUserId == driver.Id).AsNoTracking().ToListAsync();
            if (complains == null)
            {
                throw new ArgumentNullException("there no Complaints found ");
            }
            return complains;
        }



        public async Task<int> SaveChangesAsync()
        {
            return await _db.SaveChangesAsync();
        }

        

       
    }
}