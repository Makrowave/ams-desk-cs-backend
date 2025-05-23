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
public class ServicesController : ControllerBase
{
    private readonly IServicesService _servicesService;
    public ServicesController(IServicesService servicesService)
    {
        _servicesService = servicesService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Service>>> GetServices() 
    {
        var result = await _servicesService.GetServices();
        return Ok(result.Data);
    }

    [HttpPut("{id}")]
    [Authorize(Policy = "AdminAccessToken")]
    public async Task<ActionResult<ServiceDto>> PutService(short id, ServiceDto service)
    {
        var result = await _servicesService.PutService(id, service);
        if (result.Status == ServiceStatus.NotFound)
        {
            return NotFound(result.Message);
        }
        return Ok(result.Data);
    }
        
    [HttpPost]
    [Authorize(Policy = "AdminAccessToken")]
    public async Task<ActionResult<ServiceDto>> PostService(ServiceDto service)
    {
        var result = await _servicesService.PostService(service);
        return Ok(result.Data);
    }
    [HttpGet("Categories")]
    public async Task<ActionResult<IEnumerable<ServiceCategoryDto>>> GetServiceCategories()
    {
        var result = await _servicesService.GetServiceCategories();
        return Ok(result.Data);
    }
        
    [HttpGet("fromCategory/{id}")]
    public async Task<ActionResult<IEnumerable<ServiceCategoryDto>>> GetByCategory(short id)
    {
        var result = await _servicesService.GetServicesFromCategory(id);
        return Ok(result.Data);
    }
        
    [HttpDelete("{id}")]
    [Authorize(Policy = "AdminAccessToken")]
    public async Task<ActionResult<ServiceDto>> DeleteService(short id)
    {
        var result = await _servicesService.DeleteService(id);
        if (result.Status == ServiceStatus.NotFound)
        {
            return NotFound(result.Message);
        }
        return Ok();
    }
}