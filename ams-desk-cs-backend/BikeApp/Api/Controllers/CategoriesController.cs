using Microsoft.AspNetCore.Mvc;
using ams_desk_cs_backend.BikeApp.Application.Interfaces;
using ams_desk_cs_backend.BikeApp.Dtos.AppModelDto;

namespace ams_desk_cs_backend.BikeApp.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoriesService _categoriesService;

        public CategoriesController(ICategoriesService categoriesService)
        {
            _categoriesService = categoriesService;
        }

        // GET: api/Categories
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CategoryDto>>> GetCategories()
        {
            var result = await _categoriesService.GetCategories();
            return Ok(result.Data);
        }
    }
}
