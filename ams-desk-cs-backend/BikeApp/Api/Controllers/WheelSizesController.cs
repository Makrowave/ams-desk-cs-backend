using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ams_desk_cs_backend.BikeApp.Dtos.AppModelDto;
using ams_desk_cs_backend.BikeApp.Application.Interfaces;
using ams_desk_cs_backend.Shared.Results;
using ams_desk_cs_backend.BikeApp.Application.Services;

namespace ams_desk_cs_backend.BikeApp.Api.Controllers
{
    [Authorize(Policy = "AccessToken")]
    [Route("api/[controller]")]
    [ApiController]
    public class WheelSizesController : ControllerBase
    {
        private readonly IWheelSizesService _wheelSizesService;
        public WheelSizesController(IWheelSizesService wheelSizesService)
        {
            _wheelSizesService = wheelSizesService;
        }


        // GET: api/WheelSize
        [HttpGet]
        public async Task<ActionResult<IEnumerable<StatusDto>>> GetWheelSizes()
        {
            var result = await _wheelSizesService.GetWheelSizes();
            return Ok(result.Data);
        }
        [HttpPost]
        [Authorize(Policy = "AdminAccessToken")]
        public async Task<IActionResult> AddWheelSize(short wheelSize)
        {
            var result = await _wheelSizesService.PostWheelSize(wheelSize);
            if (result.Status == ServiceStatus.BadRequest)
            {
                return NotFound(result.Message);
            }
            return Ok();
        }
        [HttpDelete("{id}")]
        [Authorize(Policy = "AdminAccessToken")]
        public async Task<IActionResult> DeleteWheelSize(short id)
        {
            var result = await _wheelSizesService.DeleteWheelSize(id);
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
}
