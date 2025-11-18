using Microsoft.AspNetCore.Mvc;
using Wasalnyy.BLL.Service.Abstraction;
using Wasalnyy.DAL.Enum;

namespace Wasalnyy.PL.Controllers.Admin
{
    [Route("api/admin/trips")]
    [ApiController]
    public class AdminTripsController : ControllerBase
    {
        private readonly IAdminService _adminService;

        public AdminTripsController(IAdminService adminService)
        {
            _adminService = adminService;
        }

       

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTripById(Guid id)
        {
            var trip = await _adminService.GetTripByIdAsync(id);
            if (trip == null) return NotFound("Trip not found.");
            return Ok(trip);
        }

        [HttpGet("status/{status}")]
        public async Task<IActionResult> GetTripsByStatus(TripStatus status)
        {
            var trips = await _adminService.GetTripsByStatusAsync(status);
            return Ok(trips);
        }

   
    }
}