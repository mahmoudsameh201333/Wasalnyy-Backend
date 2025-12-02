using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Wasalnyy.DAL.Entities;


namespace Wasalnyy.DAL.Repo.Abstraction
{
    public interface IComplaintRepo
    {
    
        Task AddAsync(Complaint complaint);
        Task<Complaint> GetByIdAsync(Guid id);
        Task<IEnumerable<Complaint>> GetAllComplaintsAsync();
        Task<IEnumerable<Complaint>> GetComplaintsAgainstUserAsync(string userId);
        Task<IEnumerable<Complaint>> GetComplaintsByUserAsync(string userId);
        Task<IEnumerable<Complaint>> GetCriticalComplaintsAgainstUserAsync(string userId);
        Task<IEnumerable<Complaint>> GetNonCriticalComplaintsAgainstUserAsync(string userId);
        Task<UserComplaintStatistics> GetUserComplaintStatisticsAsync(string userId);
        Task<bool> HasComplainedAboutTripAsync(Guid tripId, string userId);
        Task UpdateComplaintStatusAsync(Guid complaintId, ComplaintStatus status);
        Task<IEnumerable<Complaint>> DriverAgainstComplains(string licen);
        Task<IEnumerable<Complaint>> DriverComplains(string licen);
        Task<int> SaveChangesAsync();
    }
}