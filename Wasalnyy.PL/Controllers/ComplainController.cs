using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Threading.Tasks;
using Wasalnyy.BLL.DTO.ReviewandReports;
using Wasalnyy.BLL.Exceptions;
using Wasalnyy.BLL.Service.Abstraction;
using Wasalnyy.DAL.Entities;
using Wasalnyy.DAL.Enum;

namespace Wasalnyy.PL.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ComplaintsController : ControllerBase
    {
        private readonly IComplaintService _complaintService;

        public ComplaintsController(IComplaintService complaintService)
        {
            _complaintService = complaintService;
        }

        /
        private string GetCurrentUserId()
        {
            return User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        }

 
        [HttpPost("add")]
        [Authorize]
        public async Task<IActionResult> SubmitComplaint([FromBody] CreateComplaintDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var currentUserId = GetCurrentUserId();
            if (string.IsNullOrEmpty(currentUserId))
                return Unauthorized("User not authenticated");

            try
            {
                var complaintId = await _complaintService.AddComplaintAsync(dto, currentUserId);
                return Ok(new { Message = "Complaint submitted successfully", complaintId });
            }
            catch (ValidationException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { Message = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid();
            }
        }


        [HttpGet("against/{userId}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetComplaintsAgainstUser(string userId)
        {
            if (string.IsNullOrEmpty(userId))
                return BadRequest("User ID is required");

            try
            {
                var complaints = await _complaintService.GetComplaintsAgainstUserAsync(userId);
                return Ok(complaints);
            }
            catch
            {
                return BadRequest("Error getting complaints");
            }
        }

        [HttpGet("against/{userId}/critical")]
        [AllowAnonymous]
        public async Task<IActionResult> GetCriticalComplaints(string userId)
        {
            if (string.IsNullOrEmpty(userId))
                return BadRequest("User ID is required");

            try
            {
                var complaints = await _complaintService.GetCriticalComplaintsAsync(userId);
                return Ok(complaints);
            }
            catch
            {
                return BadRequest("Error getting critical complaints");
            }
        }

       
        [HttpGet("against/{userId}/non-critical")]
        [AllowAnonymous]
        public async Task<IActionResult> GetNonCriticalComplaints(string userId)
        {
            if (string.IsNullOrEmpty(userId))
                return BadRequest("User ID is required");

            try
            {
                var complaints = await _complaintService.GetNonCriticalComplaintsAsync(userId);
                return Ok(complaints);
            }
            catch
            {
                return BadRequest("Error getting non-critical complaints");
            }
        }

     
        [HttpGet("my-complaints")]
        [Authorize]
        public async Task<IActionResult> GetMyComplaints()
        {
            var currentUserId = GetCurrentUserId();
            if (string.IsNullOrEmpty(currentUserId))
                return Unauthorized("User not authenticated");

            try
            {
                var complaints = await _complaintService.GetComplaintsByUserAsync(currentUserId);
                return Ok(complaints);
            }
            catch
            {
                return BadRequest("Error getting your complaints");
            }
        }

       
        [HttpGet("{complaintId:guid}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetComplaintById(Guid complaintId)
        {
            if (complaintId == Guid.Empty)
                return BadRequest("Complaint ID is required");

            try
            {
                var complaint = await _complaintService.GetComplaintByIdAsync(complaintId);
                return Ok(complaint);
            }
            catch (NotFoundException)
            {
                return NotFound("Complaint not found");
            }
        }

   
        [HttpGet("statistics/{userId}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetComplaintStatistics(string userId)
        {
            if (string.IsNullOrEmpty(userId))
                return BadRequest("User ID is required");

            try
            {
                var statistics = await _complaintService.GetComplaintStatisticsAsync(userId);
                return Ok(statistics);
            }
            catch
            {
                return BadRequest("Error getting statistics");
            }
        }

       
        [HttpGet("check-ban/{userId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CheckBanStatus(string userId)
        {
            if (string.IsNullOrEmpty(userId))
                return BadRequest("User ID is required");

            try
            {
                var statistics = await _complaintService.GetComplaintStatisticsAsync(userId);

                return Ok(new
                {
                    userId,
                    criticalComplaintsCount = statistics.CriticalComplaintsCount,
                    nonCriticalComplaintsCount = statistics.NonCriticalComplaintsCount,
                    isBanned = statistics.IsBanned,
                    needsWarning = statistics.NeedsWarning,
                    banReason = statistics.BanReason,
                    warningMessage = statistics.WarningMessage
                });
            }
            catch
            {
                return BadRequest("Error checking ban status");
            }
        }

    
        [HttpPut("{complaintId:guid}/status")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateComplaintStatus(Guid complaintId, [FromBody] UpdateComplaintStatusDto dto)
        {
            if (complaintId == Guid.Empty)
                return BadRequest("Complaint ID is required");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                await _complaintService.UpdateComplaintStatusAsync(complaintId, dto.Status);
                return Ok(new { Message = "Complaint status updated successfully" });
            }
            catch (NotFoundException)
            {
                return NotFound("Complaint not found");
            }
        }
    }

  
}