using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ams_desk_cs_backend.Shared.Results;
using ams_desk_cs_backend.BikeApp.Interfaces;
namespace ams_desk_cs_backend.BikeApp.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class WheelSizesController : ControllerBase
    {
        private readonly IWheelSizesService _wheelSizesService;
        private readonly ILogger<WheelSizesController> _logger;
        public WheelSizesController(IWheelSizesService wheelSizesService, ILogger<WheelSizesController> logger)
        {
            _wheelSizesService = wheelSizesService;
            _logger = logger;
        }


        // GET: api/WheelSize
        [HttpGet]
        [Authorize(Policy = "AccessToken")]
        public async Task<ActionResult<IEnumerable<short>>> GetWheelSizes()
        {
            var result = await _wheelSizesService.GetWheelSizes();
            return Ok(result.Data);
        }
        [HttpPost("{wheelSize}")]
        [Authorize(Policy = "AdminAccessToken")]
        public async Task<IActionResult> AddWheelSize(short wheelSize)
        {
            _logger.LogWarning(wheelSize.ToString());
            var result = await _wheelSizesService.PostWheelSize(wheelSize);
            if (result.Status == ServiceStatus.BadRequest)
            {
                return BadRequest(result.Message);
            }
            return Ok(wheelSize);
        }
        [HttpDelete("{id}")]
        [Authorize(Policy = "AdminAccessToken")]
        public async Task<IActionResult> DeleteWheelSize(short id)
        {
            var result = await _wheelSizesService.DeleteWheelSize(id);
            if (result.Status == ServiceStatus.NotFound)
            {
                return NotFound(result.Message);
            }
            if (result.Status == ServiceStatus.BadRequest)
            {
                return BadRequest(result.Message);
            }
            return Ok();
        }
    }
}
