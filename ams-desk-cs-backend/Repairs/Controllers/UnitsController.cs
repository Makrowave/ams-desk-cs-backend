using ams_desk_cs_backend.Repairs.Dtos;
using ams_desk_cs_backend.Repairs.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ams_desk_cs_backend.Repairs.Controllers;

[Authorize(Policy = "AccessToken")]
[Route("api/[controller]")]
[ApiController]
public class UnitsController : ControllerBase
{
    private readonly IUnitsService _unitsService;
    public UnitsController(IUnitsService unitsService)
    {
        _unitsService = unitsService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<UnitDto>>> GetUnits()
    {
        var units = await _unitsService.GetUnits();
        return Ok(units.Data);
    }
}