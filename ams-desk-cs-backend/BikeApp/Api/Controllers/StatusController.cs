using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ams_desk_cs_backend.BikeApp.Dtos.AppModelDto;
using ams_desk_cs_backend.BikeApp.Application.Interfaces;
using ams_desk_cs_backend.Shared.Results;

namespace ams_desk_cs_backend.BikeApp.Api.Controllers
{
    [Authorize(Policy = "AccessToken")]
    [Route("api/[controller]")]
    [ApiController]
    public class StatusController : ControllerBase
    {
        private readonly IStatusService _statusService;
        public StatusController(IStatusService statusService)
        {
            _statusService = statusService;
        }


        // GET: api/Status
        [HttpGet]
        public async Task<ActionResult<IEnumerable<StatusDto>>> GetStatuses()
        {
            var result = await _statusService.GetStatuses();
            return Ok(result.Data);
        }

        // GET: api/Status/NotSold
        [HttpGet("Excluded")]
        public async Task<ActionResult<IEnumerable<StatusDto>>> GetStatusesExcluded([FromQuery] int[] exclude)
        {
            var result = await _statusService.GetStatusesExcluded(exclude);
            return Ok(result.Data);
        }

        // GET: api/Status/5
        [HttpGet("{id}")]
        public async Task<ActionResult<StatusDto>> GetStatus(short id)
        {
            var result = await _statusService.GetStatus(id);
            if (result.Status == ServiceStatus.NotFound)
            {
                return NotFound(result.Message);
            }
            return Ok(result.Data);
        }
        [HttpPost]
        [Authorize(Policy = "AdminAccessToken")]
        public async Task<IActionResult> AddStatus(StatusDto color)
        {
            var result = await _statusService.PostStatus(color);
            if (result.Status == ServiceStatus.BadRequest)
            {
                return NotFound(result.Message);
            }
            return Ok();
        }
        [HttpPut("{id}")]
        [Authorize(Policy = "AdminAccessToken")]
        public async Task<IActionResult> UpdateStatus(short id, StatusDto color)
        {
            var result = await _statusService.UpdateStatus(id, color);
            if (result.Status == ServiceStatus.NotFound)
            {
                return NotFound(result.Message);
            }
            return Ok();
        }
        [HttpPut("ChangeOrder")]
        [Authorize(Policy = "AdminAccessToken")]
        public async Task<IActionResult> ChangeOrder(short first, short last)
        {
            var result = await _statusService.ChangeOrder(first, last);
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
        [HttpDelete("{id}")]
        [Authorize(Policy = "AdminAccessToken")]
        public async Task<IActionResult> DeleteStatus(short id)
        {
            var result = await _statusService.DeleteStatus(id);
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
