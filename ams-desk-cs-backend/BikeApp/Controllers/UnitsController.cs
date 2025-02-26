using ams_desk_cs_backend.BikeApp.Dtos.Repairs;
using ams_desk_cs_backend.BikeApp.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ams_desk_cs_backend.BikeApp.Controllers
{
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
}
