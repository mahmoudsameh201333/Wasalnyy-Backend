using Microsoft.AspNetCore.Mvc;
using Wasalnyy.BLL.Service.Abstraction;

namespace Wasalnyy.PL.Controllers.Admin
{
    [Route("api/admin/riders")]
    [ApiController]
    public class AdminRidersController : ControllerBase
    {
        private readonly IAdminService _adminService;

        public AdminRidersController(IAdminService adminService)
        {
            _adminService = adminService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllRiders()
        {
            var riders = await _adminService.GetAllRidersAsync();
            return Ok(riders);
        }

        [HttpGet("phone/{phone}")]
        public async Task<IActionResult> GetRiderByPhone(string phone)
        {
            var rider = await _adminService.GetRiderByPhoneAsync(phone);
            if (rider == null) return NotFound("Rider not found.");
            return Ok(rider);
        }

        [HttpGet("{riderId}/trips/count")]
        public async Task<IActionResult> GetRiderTripCount(string riderId)
        {
            var count = await _adminService.GetRiderTripCountAsync(riderId);
            return Ok(new { riderId, tripCount = count });
        }

        [HttpGet("{riderId}/trips")]
        public async Task<IActionResult> GetRiderTrips(string riderId)
        {
            var trips = await _adminService.GetRiderTripsAsync(riderId);
            return Ok(trips);
        }
    }
}