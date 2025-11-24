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
            return Ok();
        }

        [HttpPost("Confirm/{tripId}")]
        [Authorize(Roles = "Rider")]
        public async Task<IActionResult> ConfirmAsyncAsync([FromRoute] Guid tripId)
        {
            var riderId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (riderId == null)
                return Unauthorized();

            await _tripService.ConfirmTripAsync(riderId, tripId);
            return Ok();
        }


        [HttpPost("AcceptTrip/{tripId}")]
        [Authorize(Roles = "Driver")]
        public async Task<IActionResult> AcceptTripAsync([FromRoute] Guid tripId)
        {
            var driverId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (driverId == null)
                return Unauthorized();

            await _tripService.AcceptTripAsync(driverId, tripId);
            return Ok();
        }

        [HttpPost("StartTrip/{tripId}")]
        [Authorize(Roles = "Driver")]
        public async Task<IActionResult> StartTripAsync([FromRoute] Guid tripId)
        {
            var driverId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (driverId == null)
                return Unauthorized();

            await _tripService.StartTripAsync(driverId, tripId);
            return Ok();
        }

        [HttpPost("EndTrip/{tripId}")]
        [Authorize(Roles = "Driver")]
        public async Task<IActionResult> EndTripAsync([FromRoute] Guid tripId)
        {
            var driverId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (driverId == null)
                return Unauthorized();

            await _tripService.EndTripAsync(driverId, tripId);
            return Ok();
        }

    }
}
