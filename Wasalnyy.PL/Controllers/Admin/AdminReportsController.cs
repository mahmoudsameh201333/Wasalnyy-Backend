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
            var riders = await _adminService.GetTotalRidersAsync();
            var trips = await _adminService.GetTotalTripsAsync();

            return Ok(new
            {
                totalDrivers = drivers,
                totalRiders = riders,
                totalTrips = trips
            });
        }
    }
}