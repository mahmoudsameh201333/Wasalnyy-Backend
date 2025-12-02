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
            var driverId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (driverId == null) return Unauthorized();

            await _driverService.SetDriverAvailableAsync(driverId, coordinate);
            return Ok();
        }

        [HttpPost("SetAsUnAvailable")]
        public async Task<IActionResult> SetAsUnAvailableAsync()
        {
            var driverId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (driverId == null)
                return Unauthorized();

            await _driverService.SetDriverUnAvailableAsync(driverId);
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

        [HttpPost("AcceptTrip")]
        public async Task<IActionResult> AcceptTripAsync([FromBody] Guid tripId)
        {
            var driverId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (driverId == null) return Unauthorized();

            await _tripService.AcceptTripAsync(driverId, tripId);
            return Ok();
        }

        [HttpPost("StartTrip")]
        public async Task<IActionResult> StartTripAsync([FromBody] Guid tripId)
        {
            var driverId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (driverId == null) return Unauthorized();

            await _tripService.StartTripAsync(driverId, tripId);
            return Ok();
        }

        [HttpPost("EndTrip")]
        public async Task<IActionResult> EndTripAsync([FromBody] Guid tripId)
        {
            var driverId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (driverId == null) return Unauthorized();

            await _tripService.EndTripAsync(driverId, tripId);
            return Ok();
        }



        [HttpPut("UpdateInfo")]
        public async Task<IActionResult> UpdateDriverAsync([FromBody] DriverUpdateDto driverUpdate)
        {
            var driverId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (driverId == null) return Unauthorized();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _driverService.UpdateDriverInfo(driverId, driverUpdate);
            if (!result)
                return NotFound(new { Message = "Driver not found." });

            return Ok(new { Message = "Driver information updated successfully." });
        }



        [HttpGet("Vehicle")]
        public async Task<IActionResult> GetVehicleInfoAsync()
        {
            var driverId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (driverId == null) return Unauthorized();

            var vehicle = await _driverService.GetDriverVehicleInfoAsync(driverId);
            if (vehicle == null)
                return NotFound(new { Message = "Vehicle info not found." });

            return Ok(vehicle);
        }

        [HttpGet("Status")]
        public async Task<IActionResult> GetStatusAsync()
        {
            var driverId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (driverId == null) return Unauthorized();

            var status = await _driverService.GetDriverStatusAsync(driverId);
            return Ok(status);
        }

        [HttpGet("ProfileImage")]
        public async Task<IActionResult> GetProfileImageAsync()
        {
            var driverId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (driverId == null) return Unauthorized();

            var image = await _driverService.DriverProfileImageAsync(driverId);
            return Ok(image);
        }

        [HttpGet("Name")]
        public async Task<IActionResult> GetDriverNameAsync()
        {
            var driverId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (driverId == null) return Unauthorized();

            var name = await _driverService.DriverNameAsync(driverId);
            return Ok(name);
        }

        [HttpGet("CompletedTripsCount")]
        public async Task<IActionResult> GetTotalCompletedTripsAsync()
        {
            var driverId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (driverId == null) return Unauthorized();

            var tripsCount = await _driverService.GetTotalCompletedTripsAsync(driverId);
            return Ok(tripsCount);
        }

        [HttpGet("Profile")]
        public async Task<IActionResult> GetProfileAsync()
        {
            var driverId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (driverId == null)
                return Unauthorized();

            var driver = await _driverService.GetByIdAsync(driverId);
            return Ok(driver);
        }
    }
}
