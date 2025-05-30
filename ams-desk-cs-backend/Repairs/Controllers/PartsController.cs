﻿using ams_desk_cs_backend.Data.Models.Repairs;
using ams_desk_cs_backend.Repairs.Dtos;
using ams_desk_cs_backend.Repairs.Interfaces;
using ams_desk_cs_backend.Shared.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ams_desk_cs_backend.Repairs.Controllers;

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
    
    [HttpGet("filtered")]
    public async Task<ActionResult<IEnumerable<Part>>> GetFilteredParts(short categoryId, short typeId) 
    {
        var result = await _partsService.GetFilteredParts(categoryId, typeId);
        return Ok(result.Data);
    }

    [HttpPost]
    public async Task<ActionResult<Part>> AddPart(Part part)
    {
        var result = await _partsService.AddPart(part);
        return Ok(result.Data);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<Part>> UpdatePart(int id, Part part)
    {
        var result = await _partsService.ChangePart(id, part);
        if (result.Status == ServiceStatus.NotFound)
        {
            return NotFound(result.Message);
        }
        return Ok(result.Data);
    }

    [HttpPut("Merge")]
    public async Task<IActionResult> MergeParts(MergePartsDto dto)
    {
        var result = await _partsService.MergeParts(dto.Id1, dto.Id2, dto.Part);
        if (result.Status == ServiceStatus.NotFound)
        {
            return NotFound(result.Message);
        }
        return Ok(result.Data);
    }
    [HttpDelete("{id}")]
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