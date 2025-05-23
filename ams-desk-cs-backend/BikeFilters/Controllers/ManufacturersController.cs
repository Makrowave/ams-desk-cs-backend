﻿using ams_desk_cs_backend.BikeFilters.Dtos;
using ams_desk_cs_backend.BikeFilters.Interfaces;
using ams_desk_cs_backend.Shared.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ams_desk_cs_backend.BikeFilters.Controllers;

[Authorize(Policy = "AccessToken")]
[Route("api/[controller]")]
[ApiController]
public class ManufacturersController : ControllerBase
{
    private readonly IManufacturersService _manufacturersService;

    public ManufacturersController(IManufacturersService manufacturersService)
    {
        _manufacturersService = manufacturersService;
    }


    // GET: api/Manufacturers
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ManufacturerDto>>> GetManufacturers()
    {
        var result = await _manufacturersService.GetManufacturers();
        return Ok(result.Data);
    }

    [HttpPost]
    public async Task<ActionResult<ManufacturerDto>> AddManufacturer(ManufacturerDto manufacturer)
    {
        var result = await _manufacturersService.PostManufacturer(manufacturer);
        if (result.Status == ServiceStatus.BadRequest)
        {
            return NotFound(result.Message);
        }

        return Ok(result.Data);
    }

    [HttpPut("{id}")]
    [Authorize(Policy = "AdminAccessToken")]
    public async Task<ActionResult<ManufacturerDto>> UpdateManufacturer(short id, ManufacturerDto manufacturer)
    {
        var result = await _manufacturersService.UpdateManufacturer(id, manufacturer);
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
        var result = await _manufacturersService.ChangeOrder(first, last);
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
    public async Task<IActionResult> DeleteManufacturer(short id)
    {
        var result = await _manufacturersService.DeleteManufacturer(id);
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