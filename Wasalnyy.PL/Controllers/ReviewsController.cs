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
using Wasalnyy.DAL.Repo.Abstraction;

namespace Wasalnyy.PL.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewsController : ControllerBase
    {
        private readonly IReviewService _reviewService;

        public ReviewsController(IReviewService reviewService)
        {
            _reviewService = reviewService;
        }

        // Helper method to get current user ID from token
        private string GetCurrentUserId()
        {
            return User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        }


        [HttpPost("add")]
        [Authorize(Roles = "Driver,Rider")]
        public async Task<ActionResult<ReturnReviewDto>> AddReview([FromBody] CreateReviewDto dto)
        {
            // Check if data is valid
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Get current user
            var currentUserId = GetCurrentUserId();
            if (string.IsNullOrEmpty(currentUserId))
                return Unauthorized("User not authenticated");

            try
            {
                // Call service to add review
                var review = await _reviewService.AddReviewAsync(dto, currentUserId);
                return Ok(new { Message = "Review added successfully", review });
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
                return StatusCode(403, new { Message = ex.Message });
            }
        }


        [HttpGet("driver/{driverId}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetDriverReviews(string driverId)
        {
            if (string.IsNullOrEmpty(driverId))
                return BadRequest("Driver ID is required");

            try
            {
                var reviews = await _reviewService.GetDriverReviewsAsync(driverId);
                return Ok(reviews);
            }
            catch
            {
                return BadRequest("Error getting reviews");
            }
        }

        // GET: api/reviews/driver/{driverId}/rating
        [HttpGet("driver/{driverId}/rating")]
        [AllowAnonymous]
        public async Task<IActionResult> GetDriverRating(string driverId)
        {
            if (string.IsNullOrEmpty(driverId))
                return BadRequest("Driver ID is required");

            try
            {
                var rating = await _reviewService.GetDriverAverageRatingAsync(driverId);
                return Ok(new { driverId, averageRating = rating });
            }
            catch
            {
                return BadRequest("Error getting rating");
            }
        }

        // GET: api/reviews/driver/{driverId}/stats
        [HttpGet("driver/{driverId}/stats")]
        [AllowAnonymous]
        public async Task<IActionResult> GetDriverStats(string driverId)
        {
            if (string.IsNullOrEmpty(driverId))
                return BadRequest("Driver ID is required");

            try
            {
                var reviews = await _reviewService.GetDriverReviewsAsync(driverId);
                var rating = await _reviewService.GetDriverAverageRatingAsync(driverId);
                var stats = await _reviewService.GetReviewStatisticsAsync(driverId, ReviewerType.Driver);

                return Ok(new { driverId, rating, stats, reviews });
            }
            catch
            {
                return BadRequest("Error getting stats");
            }
        }

        // GET: api/reviews/rider/{riderId}
        [HttpGet("rider/{riderId}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetRiderReviews(string riderId)
        {
            if (string.IsNullOrEmpty(riderId))
                return BadRequest("Rider ID is required");

            try
            {
                var reviews = await _reviewService.GetRiderReviewsAsync(riderId);
                return Ok(reviews);
            }
            catch
            {
                return BadRequest("Error getting reviews");
            }
        }

        // GET: api/reviews/rider/{riderId}/rating
        [HttpGet("rider/{riderId}/rating")]
        [AllowAnonymous]
        public async Task<IActionResult> GetRiderRating(string riderId)
        {
            if (string.IsNullOrEmpty(riderId))
                return BadRequest("Rider ID is required");

            try
            {
                var rating = await _reviewService.GetRiderAverageRatingAsync(riderId);
                return Ok(new { riderId, averageRating = rating });
            }
            catch
            {
                return BadRequest("Error getting rating");
            }
        }

        // GET: api/reviews/my-reviews?reviewerType=Rider
        [HttpGet("my-reviews")]
        [Authorize]
        public async Task<IActionResult> GetMyReviews([FromQuery] ReviewerType reviewerType)
        {
            var currentUserId = GetCurrentUserId();
            if (string.IsNullOrEmpty(currentUserId))
                return Unauthorized("User not authenticated");

            try
            {
                var reviews = await _reviewService.GetMyReviewsAsync(currentUserId, reviewerType);
                return Ok(reviews);
            }
            catch
            {
                return BadRequest("Error getting your reviews");
            }
        }

        // GET: api/reviews/{reviewId:guid}
        [HttpGet("{reviewId:guid}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetReviewById(Guid reviewId)
        {
            if (reviewId == Guid.Empty)
                return BadRequest("Review ID is required");

            try
            {
                var review = await _reviewService.GetReviewByIdAsync(reviewId);
                return Ok(review);
            }
            catch (NotFoundException)
            {
                return NotFound("Review not found");
            }
        }

        // ============= UPDATE REVIEW =============

        // PUT: api/reviews/{reviewId:guid}
        [HttpPut("{reviewId:guid}")]
        [Authorize]
        public async Task<IActionResult> UpdateReview(Guid reviewId, [FromBody] UpdateReviewDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (reviewId != dto.Id)
                return BadRequest("Review ID mismatch");

            var currentUserId = GetCurrentUserId();
            if (string.IsNullOrEmpty(currentUserId))
                return Unauthorized("User not authenticated");

            try
            {
                var updated = await _reviewService.UpdateReviewAsync(dto, currentUserId);
                return Ok(new { Message = "Review updated successfully", updated });
            }
            catch (NotFoundException)
            {
                return NotFound("Review not found");
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(403, new { Message = ex.Message });
            }
            catch (ValidationException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        // ============= DELETE REVIEW =============

        // DELETE: api/reviews/{reviewId:guid}
        [HttpDelete("{reviewId:guid}")]
        [Authorize]
        public async Task<IActionResult> DeleteReview(Guid reviewId)
        {
            if (reviewId == Guid.Empty)
                return BadRequest("Review ID is required");

            var currentUserId = GetCurrentUserId();
            if (string.IsNullOrEmpty(currentUserId))
                return Unauthorized("User not authenticated");

            try
            {
                var success = await _reviewService.DeleteReviewAsync(reviewId, currentUserId);
                if (!success)
                    return NotFound("Review not found");

                return Ok(new { Message = "Review deleted successfully" });
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(403, new { Message = ex.Message });
            }
        }
    }
}