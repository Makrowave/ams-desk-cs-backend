using ams_desk_cs_backend.BikeApp.Data.Models.Repairs;
using ams_desk_cs_backend.BikeApp.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ams_desk_cs_backend.BikeApp.Controllers
{
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

        [HttpPost]
        public async Task<IActionResult> AddPart(Part part)
        {
            await _partsService.AddPart(part);
            return Ok();
        }
    }
}
