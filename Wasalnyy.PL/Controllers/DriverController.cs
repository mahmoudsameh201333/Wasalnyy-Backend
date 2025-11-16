using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Wasalnyy.BLL.DTO.Trip;
using Wasalnyy.DAL.Entities;

namespace Wasalnyy.PL.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Driver")]
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
            try
            {
                var driverId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                if (driverId == null)
                    return Unauthorized();

                await _driverService.SetDriverAvailableAsync(driverId, coordinate);
                return Ok();
            }
            catch (Exception)
            {
                Ok();
            }
            return Ok();

        }

        [HttpPost("UpdateLocation")]
        public async Task<IActionResult> UpdateLocationAsync([FromBody] Coordinates coordinate)
        {
            try
            {
                var driverId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                if (driverId == null)
                    return Unauthorized();

                await _driverService.UpdateLocationAsync(driverId, coordinate);
                return Ok();
            }
            catch (Exception)
            {
                return Ok();

            }

        }

        [HttpPost("AcceptTrip")]
        public async Task<IActionResult> AcceptTripAsync([FromBody] Guid tripId)
        {
            var driverId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (driverId == null)
                return Unauthorized();

            await _tripService.AcceptTripAsync(driverId, tripId);
            return Ok();
        }

        [HttpPost("StartTrip")]
        public async Task<IActionResult> StartTripAsync([FromBody] Guid tripId)
        {
            var driverId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (driverId == null)
                return Unauthorized();

            await _tripService.StartTripAsync(driverId, tripId);
            return Ok();
        }

        [HttpPost("EndTrip")]
        public async Task<IActionResult> EndTripAsync([FromBody] Guid tripId)
        {
            var driverId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (driverId == null)
                return Unauthorized();

            await _tripService.EndTripAsync(driverId, tripId);
            return Ok();
        }
    }
}
