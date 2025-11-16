using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Wasalnyy.BLL.DTO.Zone;

namespace Wasalnyy.PL.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
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
