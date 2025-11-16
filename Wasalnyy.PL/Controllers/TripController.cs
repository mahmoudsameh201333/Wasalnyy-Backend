using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Wasalnyy.BLL.DTO.Trip;
using Wasalnyy.BLL.DTO.Zone;

namespace Wasalnyy.PL.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Rider")]
    public class TripController : ControllerBase
    {
        private readonly ITripService _tripService;

        public TripController(ITripService tripService)
        {
            _tripService = tripService;
        }
        [HttpPost("Request")]
        public async Task<IActionResult> RequestAsyncAsync([FromForm] RequestTripDto dto)
        {
            var riderId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (riderId == null)
                return Unauthorized();

            await _tripService.RequestTripAsync(riderId, dto);
            return Created();
        }

        
    }
}
