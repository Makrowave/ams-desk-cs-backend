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
        [HttpGet("NotSold")]
        public async Task<ActionResult<IEnumerable<StatusDto>>> GetStatusesNotSold()
        {
            var result = await _statusService.GetStatusesNotSold();
            return Ok(result.Data);
        }

        // GET: api/Status/5
        [HttpGet("{id}")]
        public async Task<ActionResult<StatusDto>> GetStatus(short id)
        {
            var result = await _statusService.GetStatus(id);
            if(result.Status == ServiceStatus.NotFound)
            {
                return NotFound(result.Message);
            }
            return Ok(result.Data);
        }
    }
}
