using Microsoft.AspNetCore.Mvc;
using Wasalnyy.BLL.Service.Abstraction;

namespace Wasalnyy.PL.Controllers.Admin
{
    [Route("api/admin/reports")]
    [ApiController]
    public class AdminReportsController : ControllerBase
    {
        private readonly IAdminService _adminService;

        public AdminReportsController(IAdminService adminService)
        {
            _adminService = adminService;
        }

        [HttpGet("totals")]
        public async Task<IActionResult> GetTotalCounts()
        {
            var drivers = await _adminService.GetTotalDriversAsync();
            
            var trips = await _adminService.GetTotalTripsAsync();

            return Ok(new
            {
                totalDrivers = drivers,
                
                totalTrips = trips
            });
        }

        [HttpGet("riders/count")]
        public async Task<IActionResult> GetRidersCount()
        {
            var count = await _adminService.GetRidersCount();
            return Ok(new { totalRiders = count });
        }

        [HttpGet("complaints/{id}")]
        public async Task<IActionResult> GetDriverComplaintById(Guid id)
        {
            var complaint = await _adminService.GetDriverComplainByComplainsIdAsync(id);
            if (complaint == null) return NotFound();
            return Ok(complaint);
        }
    }
}