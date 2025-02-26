using ams_desk_cs_backend.BikeApp.Dtos.Repairs;
using ams_desk_cs_backend.BikeApp.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ams_desk_cs_backend.BikeApp.Controllers
{
    [Authorize(Policy = "AccessToken")]
    [Route("api/[controller]")]
    [ApiController]
    public class PartCategoriesController : ControllerBase
    {
        private readonly IPartCategoriesService _partCategoriesService;
        public PartCategoriesController(IPartCategoriesService partCategoriesService)
        {
            _partCategoriesService = partCategoriesService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PartCategoryDto>>> GetCategories()
        {
            var result = await _partCategoriesService.GetPartCategories();
            return Ok(result.Data);
        }
    }
}
