﻿using ams_desk_cs_backend.BikeFilters.Dtos;
using ams_desk_cs_backend.BikeFilters.Interfaces;
using ams_desk_cs_backend.Shared.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ams_desk_cs_backend.BikeFilters.Controllers;

[Authorize(Policy = "AccessToken")]
[Route("api/[controller]")]
[ApiController]
public class ColorsController : ControllerBase
{
    private readonly IColorsService _colorsService;

    public ColorsController(IColorsService colorsService)
    {
        _colorsService = colorsService;
    }

    // GET: api/Colors
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ColorDto>>> GetColors()
    {
        var result = await _colorsService.GetColors();
        return Ok(result.Data);
    }
    // GET: api/Colors/5
    [HttpGet("{id}")]
    public async Task<ActionResult<ColorDto>> GetColor(short id)
    {
        var result = await _colorsService.GetColor(id);
        if (result.Status == ServiceStatus.NotFound)
        {
            return NotFound(result.Message);
        }
        return Ok(result.Data);
    }
    [HttpPost]
    [Authorize(Policy = "AdminAccessToken")]
    public async Task<ActionResult<ColorDto>> AddColor(ColorDto color)
    {
        var result = await _colorsService.PostColor(color);
        if (result.Status == ServiceStatus.BadRequest)
        {
            return NotFound(result.Message);
        }
        return Ok(result.Data);
    }
    [HttpPut("{id}")]
    [Authorize(Policy = "AdminAccessToken")]
    public async Task<ActionResult<ColorDto> >UpdateColor(short id, ColorDto color)
    {
        var result = await _colorsService.UpdateColor(id, color);
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
        var result = await _colorsService.ChangeOrder(first, last);
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
    public async Task<IActionResult> DeleteColor(short id)
    {
        var result = await _colorsService.DeleteColor(id);
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