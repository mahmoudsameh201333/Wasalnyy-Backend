using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Wasalnyy.BLL.DTO.ReviewandReports;
using Wasalnyy.DAL.Entities;

namespace Wasalnyy.BLL.Service.Abstraction
{
    public interface IComplaintService
    {
        
        Task<Guid> AddComplaintAsync(CreateComplaintDto dto, string currentUserId);
        Task<IEnumerable<ReturnComplaintDto>> GetComplaintsAgainstUserAsync(string userId);
        Task<IEnumerable<ReturnComplaintDto>> GetComplaintsByUserAsync(string userId);
        Task<IEnumerable<ReturnComplaintDto>> GetCriticalComplaintsAsync(string userId);
        Task<IEnumerable<ReturnComplaintDto>> GetNonCriticalComplaintsAsync(string userId);
        Task<ReturnComplaintDto> GetComplaintByIdAsync(Guid complaintId);
        Task<ComplaintStatisticsDto> GetComplaintStatisticsAsync(string userId);
        bool ShouldBeBanned(UserComplaintStatistics stats);
        bool NeedsWarning(UserComplaintStatistics stats);
        string GetBanReason(UserComplaintStatistics stats);
        string GetWarningMessage(UserComplaintStatistics stats);
        Task UpdateComplaintStatusAsync(Guid complaintId, ComplaintStatus status);
    }
}