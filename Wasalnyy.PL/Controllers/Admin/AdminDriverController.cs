using Microsoft.AspNetCore.Mvc;
using Wasalnyy.BLL.Service.Abstraction;

namespace Wasalnyy.PL.Controllers.Admin
{
    [Route("api/admin/drivers")]
    [ApiController]
    public class AdminDriversController : ControllerBase
    {
        private readonly IAdminService _adminService;

        public AdminDriversController(IAdminService adminService)
        {
            _adminService = adminService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllDrivers()
        {
            var drivers = await _adminService.GetAllDriversAsync();
            return Ok(drivers);
        }

        [HttpGet("license/{license}")]
        public async Task<IActionResult> GetDriverByLicense(string license)
        {
            var driver = await _adminService.GetDriverByLicenseAsync(license);
            if (driver == null) return NotFound("Driver not found.");
            return Ok(driver);
        }

        [HttpGet("{driverId}/trips/count")]
        public async Task<IActionResult> GetDriverTripCount(string driverId)
        {
            var count = await _adminService.GetDriverTripCountAsync(driverId);
            return Ok(new { driverId, tripCount = count });
        }

        [HttpGet("{driverId}/trips")]
        public async Task<IActionResult> GetDriverTrips(string driverId)
        {
            var trips = await _adminService.GetDriverTripsAsync(driverId);
            return Ok(trips);
        }
    }
}