﻿using ams_desk_cs_backend.Data.Models;
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
    public async Task<ActionResult<IEnumerable<Place>>> GetPlaces()
    {
        var result = await _placeService.GetPlaces();
        return Ok(result.Data);
    }
    [HttpPut("ChangeOrder")]
    [Authorize(Policy = "AdminAccessToken")]
    public async Task<IActionResult> ChangeOrder(short first, short last)
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
        return Ok();
    }
}