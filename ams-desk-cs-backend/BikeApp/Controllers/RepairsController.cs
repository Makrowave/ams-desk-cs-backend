using ams_desk_cs_backend.BikeApp.Dtos.Repairs;
using ams_desk_cs_backend.BikeApp.Interfaces;
using ams_desk_cs_backend.Shared.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ams_desk_cs_backend.BikeApp.Controllers
{
    [Authorize(Policy = "AccessToken")]
    [Route("api/[controller]")]
    [ApiController]
    public class RepairsController : ControllerBase
    {
        private readonly IRepairsService _repairsService;

        public RepairsController(IRepairsService repairsService)
        {
            _repairsService = repairsService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ShortRepairDto>>> GetRepairs(short place,
            [FromQuery] short[] excluded)
        {
            var result = await _repairsService.GetRepairs(place, excluded);
            return Ok(result.Data);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<RepairDto>> GetRepair(short id)
        {
            var result = await _repairsService.GetRepair(id);
            if (result.Status == ServiceStatus.NotFound)
            {
                return NotFound(result.Message);
            }

            return Ok(result.Data);
        }

        [HttpPost]
        public async Task<IActionResult> NewRepair(NewRepairDto newRepair)
        {
            var result = await _repairsService.CreateRepair(newRepair);
            return Ok(result.Data);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<RepairDto>> UpdateRepair(int id, RepairDto newRepair)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _repairsService.UpdateRepair(id, newRepair);
            if (result.Status == ServiceStatus.NotFound)
            {
                return NotFound(result.Message);
            }

            return Ok(result.Data);
        }

        [HttpPut("Status/{id}")]
        public async Task<ActionResult<RepairDto>> UpdateRepairStatus(int id, short statusId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _repairsService.UpdateStatus(id, statusId);
            return result.Status switch
            {
                ServiceStatus.NotFound => NotFound(result.Message),
                ServiceStatus.Ok => Ok(result.Data),
                _ => BadRequest(result.Message)
            };
        }

        [HttpPut("Employee/{id}")]
        public async Task<ActionResult<RepairDto>> UpdateRepairEmployee(int id, short employeeId, bool collection)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _repairsService.UpdateEmployee(id, employeeId, collection);
            return result.Status switch
            {
                ServiceStatus.NotFound => NotFound(result.Message),
                ServiceStatus.Ok => Ok(result.Data),
                _ => BadRequest(result.Message)
            };
        }
    }
}