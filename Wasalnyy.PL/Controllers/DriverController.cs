using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Wasalnyy.BLL.DTO.Driver;
using Wasalnyy.BLL.DTO.Trip;
using Wasalnyy.DAL.Entities;
using Wasalnyy.PL.Filters;

namespace Wasalnyy.PL.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Driver")]
    [ServiceFilter(typeof(WasalnyyOnlineActionFilter))]

    public class DriverController : ControllerBase
    {
        private readonly IDriverService _driverService;
        private readonly ITripService _tripService;

        public DriverController(IDriverService driverService, ITripService tripService)
        {
            _driverService = driverService;
            _tripService = tripService;
        }

   

        [HttpPost("SetAsAvailable")]
        public async Task<IActionResult> SetAsAvailableAsync([FromBody] Coordinates coordinate)
        {
            var driverId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (driverId == null) return Unauthorized();

            await _driverService.SetDriverAvailableAsync(driverId, coordinate);
            return Ok();
        }

        [HttpPost("UpdateLocation")]
        public async Task<IActionResult> UpdateLocationAsync([FromBody] Coordinates coordinate)
        {
            var driverId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (driverId == null) return Unauthorized();

            await _driverService.UpdateLocationAsync(driverId, coordinate);
            return Ok();
        }

        
    }
}
