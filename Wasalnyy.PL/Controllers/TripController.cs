using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Wasalnyy.BLL.DTO.Trip;
using Wasalnyy.BLL.DTO.Zone;
using Wasalnyy.PL.Filters;

namespace Wasalnyy.PL.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ServiceFilter(typeof(WasalnyyOnlineActionFilter))]

    public class TripController : ControllerBase
    {
        private readonly ITripService _tripService;

        public TripController(ITripService tripService)
        {
            _tripService = tripService;
        }

        [HttpPost("Request")]
        [Authorize(Roles = "Rider")]
        public async Task<IActionResult> RequestAsyncAsync([FromForm] RequestTripDto dto)
        {
            var riderId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (riderId == null)
                return Unauthorized();

            await _tripService.RequestTripAsync(riderId, dto);
            return Created();
        }


        [HttpPost("AcceptTrip")]
        [Authorize(Roles = "Driver")]
        public async Task<IActionResult> AcceptTripAsync([FromBody] Guid tripId)
        {
            var driverId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (driverId == null)
                return Unauthorized();

            await _tripService.AcceptTripAsync(driverId, tripId);
            return Ok();
        }

        [HttpPost("StartTrip")]
        [Authorize(Roles = "Driver")]
        public async Task<IActionResult> StartTripAsync([FromBody] Guid tripId)
        {
            var driverId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (driverId == null)
                return Unauthorized();

            await _tripService.StartTripAsync(driverId, tripId);
            return Ok();
        }

        [HttpPost("EndTrip")]
        [Authorize(Roles = "Driver")]
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
