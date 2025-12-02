namespace Wasalnyy.PL.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize("Admin")]
    public class ZoneController : ControllerBase
    {
        private readonly IZoneService _zoneService;
        public ZoneController(IZoneService zoneService)
        {
            _zoneService = zoneService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAsync() 
        {
            return Ok(await _zoneService.GetAllAsync());
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync(CreateZoneDto dto)
        {
            await _zoneService.CreateZoneAsync(dto);
            return Created();
        }
    }
}
