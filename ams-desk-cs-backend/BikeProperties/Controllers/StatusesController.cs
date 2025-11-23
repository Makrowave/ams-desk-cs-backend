using ams_desk_cs_backend.BikeFilters.Dtos;
using ams_desk_cs_backend.BikeFilters.Interfaces;
using ams_desk_cs_backend.Shared.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ams_desk_cs_backend.BikeFilters.Controllers;

[Authorize(Policy = "AccessToken")]
[Route("api/[controller]")]
[ApiController]
public class StatusesController : ControllerBase
{
    private readonly IStatusService _statusService;
    public StatusesController(IStatusService statusService)
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
    public async Task<ActionResult<StatusDto>> AddStatus(StatusDto color)
    {
        var result = await _statusService.PostStatus(color);
        if (result.Status == ServiceStatus.BadRequest)
        {
            return NotFound(result.Message);
        }
        return Ok(result.Data);
    }
    [HttpPut("{id}")]
    [Authorize(Policy = "AdminAccessToken")]
    public async Task<ActionResult<StatusDto>> UpdateStatus(short id, StatusDto color)
    {
        var result = await _statusService.UpdateStatus(id, color);
        if (result.Status == ServiceStatus.NotFound)
        {
            return NotFound(result.Message);
        }
        return Ok(result.Data);
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
        return Ok(result.Data);
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