using ams_desk_cs_backend.BikeApp.Data.Models.Repairs;
using ams_desk_cs_backend.BikeApp.Interfaces;
using ams_desk_cs_backend.Shared.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ams_desk_cs_backend.BikeApp.Controllers;

[Authorize(Policy = "AccessToken")]
[Route("api/[controller]")]
[ApiController]
public class PartsController : ControllerBase
{
    private readonly IPartsService _partsService;
    public PartsController(IPartsService partsService)
    {
        _partsService = partsService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Part>>> GetParts() 
    {
        var result = await _partsService.GetParts();
        return Ok(result.Data);
    }

    [HttpPost]
    public async Task<IActionResult> AddPart(Part part)
    {
        await _partsService.AddPart(part);
        return Ok();
    }

    [HttpPut("{id}")]
    [Authorize(Policy = "AdminAccessToken")]
    public async Task<ActionResult<Part>> UpdatePart(int id, Part part)
    {
        var result = await _partsService.ChangePart(id, part);
        if (result.Status == ServiceStatus.NotFound)
        {
            return NotFound(result.Message);
        }
        return Ok(result.Data);
    }
    [HttpDelete("{id}")]
    [Authorize(Policy = "AdminAccessToken")]
    public async Task<ActionResult<Part>> DeletePart(int id)
    {
        var result = await _partsService.DeletePart(id);
        if (result.Status == ServiceStatus.NotFound)
        {
            return NotFound(result.Message);
        }
        return Ok();
    }
}