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

        
        // we can add  these both methods 
        //[HttpGet("driver/{driverId}")]
        //public async Task<IActionResult> GetTripsByDriver(string driverId)
        //{
        //    var trips = await _adminService.GetTripsByDriverAsync(driverId);
        //    return Ok(trips);
        //}

        //[HttpGet("rider/{riderId}")]
        //public async Task<IActionResult> GetTripsByRider(string riderId)
        //{
        //    var trips = await _adminService.GetTripsByRiderAsync(riderId);
        //    return Ok(trips);
        //}


    }
}