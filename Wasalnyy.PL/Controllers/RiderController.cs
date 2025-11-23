using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Wasalnyy.BLL.DTO.Rider;
using Wasalnyy.BLL.Service.Abstraction;

namespace Wasalnyy.PL.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Rider")]
    public class RiderController : ControllerBase
    {
        private readonly IRiderService _riderService;

        public RiderController(IRiderService riderService)
        {
            _riderService = riderService;
        }

      

        [HttpGet("Profile")]
        public async Task<IActionResult> GetProfileAsync()
        {
            var riderId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (riderId == null) return Unauthorized();

            var rider = await _riderService.GetByIdAsync(riderId);
            return Ok(rider);
        }

        [HttpGet("Name")]
        public async Task<IActionResult> GetNameAsync()
        {
            var riderId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (riderId == null) return Unauthorized();

            var name = await _riderService.RiderName(riderId);
            return Ok(name);
        }

        [HttpGet("ProfileImage")]
        public async Task<IActionResult> GetProfileImageAsync()
        {
            var riderId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (riderId == null) return Unauthorized();

            var image = await _riderService.RiderProfileImage(riderId);
            return Ok(image);
        }

        [HttpGet("TripsCount")]
        public async Task<IActionResult> GetTripsCountAsync()
        {
            var riderId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (riderId == null) return Unauthorized();

            var count = await _riderService.RiderTotalTrips(riderId);
            return Ok(count);
        }

        [HttpGet("IsSuspended")]
        public async Task<IActionResult> IsSuspendedAsync()
        {
            var riderId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (riderId == null) return Unauthorized();

            var suspended = await _riderService.IsRiderSuspended(riderId);
            return Ok(suspended);
        }

   

        [HttpPut("UpdateInfo")]
        public async Task<IActionResult> UpdateRider([FromBody] RiderUpdateDto riderUpdate)
        {
            var riderId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (riderId == null) return Unauthorized();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var updated = await _riderService.UpdateRiderInfo(riderId, riderUpdate);
            if (!updated)
                return NotFound(new { Message = "Rider not found." });

            return Ok(new { Message = "Rider information updated successfully." });
        }
    }
}
