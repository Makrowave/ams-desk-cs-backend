﻿
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ams_desk_cs_backend.BikeApp.Dtos.AppModelDto;
using ams_desk_cs_backend.Shared.Results;
using ams_desk_cs_backend.BikeApp.Dtos;
using ams_desk_cs_backend.BikeApp.Interfaces;

namespace ams_desk_cs_backend.BikeApp.Controllers;

[Authorize(Policy = "AccessToken")]
[Route("api/[controller]")]
[ApiController]
public class BikesController : ControllerBase
{
    private readonly IBikesService _bikesService;

    public BikesController(IBikesService bikesService)
    {
        _bikesService = bikesService;
    }

    // GET: api/Bikes
    [HttpGet("bikesByModelId/{modelId}")]
    public async Task<ActionResult<IEnumerable<BikeSubRecordDto>>> GetBikes(int modelId, short placeId)
    {
        var result = await _bikesService.GetBikes(modelId, placeId);
        if (result.Status == ServiceStatus.NotFound)
        {
            return NotFound(result.Message);
        }
        return Ok(result.Data);
    }

    // PUT: api/Bikes/5
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPut("{id}")]
    public async Task<ActionResult<BikeSubRecordDto>> PutBike(int id, BikeDto bike)
    {
        var result = await _bikesService.PutBike(id, bike);
        if (result.Status == ServiceStatus.NotFound)
        {
            return NotFound(result.Message);
        }
        return Ok(result.Data);
    }

    // POST: api/Bikes
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPost]
    public async Task<ActionResult<BikeSubRecordDto>> PostBike(BikeDto bike)
    {
        var result = await _bikesService.PostBike(bike);
        if (result.Status == ServiceStatus.BadRequest)
        {
            return BadRequest(result.Message);
        }
        return Ok(result.Data);
    }
    // DELETE: api/Bikes/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteBike(int id)
    {
        var result = await _bikesService.DeleteBike(id);
        if (result.Status == ServiceStatus.NotFound)
        {
            return NotFound(result.Message);
        }
        return NoContent();
    }

    [HttpPut("sell/{id}")]
    public async Task<IActionResult> SellBike(int id, int price)
    {
        var result = await _bikesService.SellBike(id, price);
        return result.Status switch
        {
            ServiceStatus.Ok => Ok(new {PlaceId = result.Data.PlaceId, ModelId = result.Data.ModelId}),
            ServiceStatus.NotFound => NotFound(result.Message),
            _ => BadRequest(result.Message)
        };
    }
}