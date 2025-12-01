using AutoMapper;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Wasalnyy.BLL.DTO.ReviewandReports;
using Wasalnyy.BLL.DTO.Trip;
using Wasalnyy.BLL.Exceptions;
using Wasalnyy.BLL.Validators;
using Wasalnyy.DAL.Entities;
using Wasalnyy.DAL.Enum;
using Wasalnyy.DAL.Repo.Abstraction;

namespace Wasalnyy.BLL.Service.Implementation
{
    public class ComplaintService : IComplaintService
    {
        private readonly IComplaintRepo _complaintRepo;
        private readonly ITripService _tripService;
        private readonly IMapper _mapper;
       

        // Ban thresholds( twdi7 5 msg not critical ban 3 cric ban)
        private const int CRITICAL_BAN_THRESHOLD = 3;
        private const int NON_CRITICAL_BAN_THRESHOLD = 5;
        private const int CRITICAL_WARNING_THRESHOLD = 2;
        private const int NON_CRITICAL_WARNING_THRESHOLD = 3;

        public ComplaintService(
            IComplaintRepo complaintRepo,
            ITripService tripService,
            IMapper mapper
            )
        {
            _complaintRepo = complaintRepo ;
            _tripService = tripService ;
            _mapper = mapper;
           
        }

       
        public async Task<Guid> AddComplaintAsync(CreateComplaintDto dto, string currentUserId)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto), "Complaint DTO cannot be null");

            if (string.IsNullOrEmpty(currentUserId))
                throw new ArgumentNullException(nameof(currentUserId), "Current user ID cannot be null or empty");

          

            // Get trip
            var tripDto = await _tripService.GetByIdAsync(dto.TripId);
            if (tripDto == null)
                throw new NotFoundException("Trip not found");

           
            if (tripDto.TripStatus != "Ended")
                throw new ValidationException("You can only complain about completed trips");

            
            ValidateUserInTrip(tripDto, currentUserId, dto.ComplainerType);

            
            var hasComplained = await _complaintRepo.HasComplainedAboutTripAsync(dto.TripId, currentUserId);
            if (hasComplained)
                throw new ValidationException("You have already complained about this trip");

            
            var complaint = new Complaint
            {
                Id = Guid.NewGuid(),
                TripId = dto.TripId,
                SubmittedById = currentUserId,
                AgainstUserId = GetAgainstUserId(tripDto, dto.ComplainerType),
                Description = dto.Description?.Trim(),
                Category = dto.Category,
                ComplainerType = dto.ComplainerType,
                Status = ComplaintStatus.Pending,
                CreatedAt = DateTime.UtcNow
            };

            await _complaintRepo.AddAsync(complaint);
            await _complaintRepo.SaveChangesAsync();

            return complaint.Id;
        }

        
        public async Task<IEnumerable<ReturnComplaintDto>> GetComplaintsAgainstUserAsync(string userId)
        {
            if (string.IsNullOrEmpty(userId))
                throw new ArgumentNullException(nameof(userId), "User ID cannot be null or empty");

            var complaints = await _complaintRepo.GetComplaintsAgainstUserAsync(userId);
            return _mapper.Map<IEnumerable<ReturnComplaintDto>>(complaints);
        }

      
        public async Task<IEnumerable<ReturnComplaintDto>> GetComplaintsByUserAsync(string userId)
        {
            if (string.IsNullOrEmpty(userId))
                throw new ArgumentNullException(nameof(userId), "User ID cannot be null or empty");

            var complaints = await _complaintRepo.GetComplaintsByUserAsync(userId);
            return _mapper.Map<IEnumerable<ReturnComplaintDto>>(complaints);
        }

        
        public async Task<IEnumerable<ReturnComplaintDto>> GetCriticalComplaintsAsync(string userId)
        {
            if (string.IsNullOrEmpty(userId))
                throw new ArgumentNullException(nameof(userId), "User ID cannot be null or empty");

            var complaints = await _complaintRepo.GetCriticalComplaintsAgainstUserAsync(userId);
            return _mapper.Map<IEnumerable<ReturnComplaintDto>>(complaints);
        }

       
        public async Task<IEnumerable<ReturnComplaintDto>> GetNonCriticalComplaintsAsync(string userId)
        {
            if (string.IsNullOrEmpty(userId))
                throw new ArgumentNullException(nameof(userId), "User ID cannot be null or empty");

            var complaints = await _complaintRepo.GetNonCriticalComplaintsAgainstUserAsync(userId);
            return _mapper.Map<IEnumerable<ReturnComplaintDto>>(complaints);
        }

       
        public async Task<ReturnComplaintDto> GetComplaintByIdAsync(Guid complaintId)
        {
            if (complaintId == Guid.Empty)
                throw new ArgumentNullException(nameof(complaintId), "Complaint ID cannot be empty");

            var complaint = await _complaintRepo.GetByIdAsync(complaintId);
            if (complaint == null)
                throw new NotFoundException("Complaint not found");

            return _mapper.Map<ReturnComplaintDto>(complaint);
        }

        //  COMPLAINT STATISTICS & BAN LOGIC 

       
        public async Task<ComplaintStatisticsDto> GetComplaintStatisticsAsync(string userId)
        {
            if (string.IsNullOrEmpty(userId))
                throw new ArgumentNullException(nameof(userId), "User ID cannot be null or empty");

            var stats = await _complaintRepo.GetUserComplaintStatisticsAsync(userId);

            return new ComplaintStatisticsDto
            {
                UserId = userId,
                CriticalComplaintsCount = stats.CriticalComplaintsCount,
                NonCriticalComplaintsCount = stats.NonCriticalComplaintsCount,
                IsBanned = ShouldBeBanned(stats),
                NeedsWarning = NeedsWarning(stats),
                BanReason = GetBanReason(stats),
                WarningMessage = GetWarningMessage(stats)
            };
        }

        
        public bool ShouldBeBanned(UserComplaintStatistics stats)
        {
            if (stats == null)
                return false;

            return stats.CriticalComplaintsCount >= CRITICAL_BAN_THRESHOLD ||
                   stats.NonCriticalComplaintsCount >= NON_CRITICAL_BAN_THRESHOLD;
        }

      
        public bool NeedsWarning(UserComplaintStatistics stats)
        {
            if (stats == null)
                return false;

            return stats.CriticalComplaintsCount >= CRITICAL_WARNING_THRESHOLD ||
                   stats.NonCriticalComplaintsCount >= NON_CRITICAL_WARNING_THRESHOLD;
        }

        
        public string GetBanReason(UserComplaintStatistics stats)
        {
            if (stats == null)
                return "No complaints";

            if (stats.CriticalComplaintsCount >= CRITICAL_BAN_THRESHOLD)
                return $"Too many critical complaints: {stats.CriticalComplaintsCount} (limit: {CRITICAL_BAN_THRESHOLD})";

            if (stats.NonCriticalComplaintsCount >= NON_CRITICAL_BAN_THRESHOLD)
                return $"Too many complaints: {stats.NonCriticalComplaintsCount} (limit: {NON_CRITICAL_BAN_THRESHOLD})";

            return "No ban reason";
        }

        
        public string GetWarningMessage(UserComplaintStatistics stats)
        {
            if (stats == null || !NeedsWarning(stats))
                return null;

            var warnings = new List<string>();

            if (stats.CriticalComplaintsCount >= CRITICAL_WARNING_THRESHOLD)
                warnings.Add($"You have {stats.CriticalComplaintsCount} critical complaints (ban at {CRITICAL_BAN_THRESHOLD})");

            if (stats.NonCriticalComplaintsCount >= NON_CRITICAL_WARNING_THRESHOLD)
                warnings.Add($"You have {stats.NonCriticalComplaintsCount} complaints (ban at {NON_CRITICAL_BAN_THRESHOLD})");

            return string.Join(" | ", warnings);
        }

        
        public async Task UpdateComplaintStatusAsync(Guid complaintId, ComplaintStatus status)
        {
            if (complaintId == Guid.Empty)
                throw new ArgumentNullException(nameof(complaintId), "Complaint ID cannot be empty");

            try
            {
                await _complaintRepo.UpdateComplaintStatusAsync(complaintId, status);
                await _complaintRepo.SaveChangesAsync();
            }
            catch (InvalidOperationException ex)
            {
                throw new NotFoundException(ex.Message);
            }
        }
        // validate methods(don't forget to remove them to vali file)
      
        private void ValidateUserInTrip(TripDto tripDto, string currentUserId, ReviewerType complainerType)
        {
            if (complainerType == ReviewerType.Rider)
            {
                if (tripDto.RiderId != currentUserId)
                    throw new UnauthorizedAccessException("Only the rider of this trip can file complaints");
            }
            else if (complainerType == ReviewerType.Driver)
            {
                if (tripDto.DriverId != currentUserId)
                    throw new UnauthorizedAccessException("Only the driver of this trip can file complaints");
            }
        }

        
        private string GetAgainstUserId(TripDto tripDto, ReviewerType complainerType)
        {
            // If rider is complaining, complaint is against driver
            if (complainerType == ReviewerType.Rider)
                return tripDto.DriverId;

            // If driver is complaining, complaint is against rider
            return tripDto.RiderId;
        }
    }
}