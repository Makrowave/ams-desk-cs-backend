using Microsoft.AspNetCore.Mvc;
using ams_desk_cs_backend.BikeApp.Application.Interfaces;
using ams_desk_cs_backend.BikeApp.Dtos.AppModelDto;
using ams_desk_cs_backend.BikeApp.Application.Services;
using ams_desk_cs_backend.Shared.Results;
using Microsoft.AspNetCore.Authorization;

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
        [Authorize(Policy = "AccessToken")]
        public async Task<ActionResult<IEnumerable<CategoryDto>>> GetCategories()
        {
            var result = await _categoriesService.GetCategories();
            return Ok(result.Data);
        }
        [HttpPost]
        [Authorize(Policy = "AdminAccessToken")]
        public async Task<IActionResult> AddCategory(CategoryDto category)
        {
            var result = await _categoriesService.PostCategory(category);
            if (result.Status == ServiceStatus.BadRequest)
            {
                return NotFound(result.Message);
            }
            return Ok();
        }
        [HttpPut("{id}")]
        [Authorize(Policy = "AdminAccessToken")]
        public async Task<IActionResult> UpdateCategory(short id, CategoryDto category)
        {
            var result = await _categoriesService.UpdateCategory(id, category);
            if (result.Status == ServiceStatus.NotFound)
            {
                return NotFound(result.Message);
            }
            return Ok();
        }
        [HttpPut("ChangeOrder")]
        [Authorize(Policy = "AdminAccessToken")]
        public async Task<IActionResult> ChangeOrder(short first, short last)
        {
            var result = await _categoriesService.ChangeOrder(first, last);
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
        [HttpDelete("{id}")]
        [Authorize(Policy = "AdminAccessToken")]
        public async Task<IActionResult> DeleteCategory(short id)
        {
            var result = await _categoriesService.DeleteCategory(id);
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
