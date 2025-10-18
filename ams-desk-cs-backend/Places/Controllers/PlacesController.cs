using ams_desk_cs_backend.Data.Models;
using ams_desk_cs_backend.Places.Dtos;
using ams_desk_cs_backend.Places.Interfaces;
using ams_desk_cs_backend.Shared.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ams_desk_cs_backend.Places.Controllers;

[Authorize(Policy = "AccessToken")]
[Route("api/[controller]")]
[ApiController]
public class PlacesController : ControllerBase
{
    private readonly IPlacesService _placeService;

    public PlacesController(IPlacesService placeService)
    {
        _placeService = placeService;
    }

    // GET: api/Places
    [HttpGet]
    public async Task<ActionResult<IEnumerable<PlaceDto>>> GetPlaces()
    {
        var result = await _placeService.GetPlaces();
        return Ok(result.Data);
    }
    [HttpGet("NotStorage")]
    public async Task<ActionResult<IEnumerable<PlaceDto>>> GetNotStorage()
    {
        var result = await _placeService.GetPlacesNotStorage();
        return Ok(result.Data);
    }

    
    [HttpPost]
    [Authorize(Policy = "AdminAccessToken")]
    public async Task<ActionResult<PlaceDto>> PostPlace(PlaceDto place)
    {
        var result = await _placeService.PostPlace(place);
        
        return Ok(result.Data);
    }
    
    [HttpPut("{id}")]
    [Authorize(Policy = "AdminAccessToken")]
    public async Task<ActionResult<PlaceDto>> PutPlace(short id, PlaceDto place)
    {
        var result = await _placeService.PutPlace(id, place);
        if (result.Status == ServiceStatus.NotFound)
        {
            return NotFound(result.Message);
        }
        return Ok(result.Data);
    }
    [HttpDelete("{id}")]
    [Authorize(Policy = "AdminAccessToken")]
    public async Task<ActionResult<PlaceDto>> DeletePlace(short id)
    {
        var result = await _placeService.DeletePlace(id);
        if (result.Status == ServiceStatus.NotFound)
        {
            return NotFound(result.Message);
        }
        return Ok();
    }
    [HttpPut("ChangeOrder")]
    [Authorize(Policy = "AdminAccessToken")]
    public async Task<ActionResult<List<PlaceDto>>> ChangeOrder(short first, short last)
    {
        var result = await _placeService.ChangeOrder(first, last);
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
}