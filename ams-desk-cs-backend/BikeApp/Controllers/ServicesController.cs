using ams_desk_cs_backend.BikeApp.Data.Models.Repairs;
using ams_desk_cs_backend.BikeApp.Dtos.Repairs;
using ams_desk_cs_backend.BikeApp.Interfaces;
using ams_desk_cs_backend.Shared.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ams_desk_cs_backend.BikeApp.Controllers
{
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
        public async Task<ActionResult<Service>> PutService(short id, Service service)
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
        public async Task<ActionResult<Service>> PostService(Service service)
        {
            await _servicesService.PostService(service);
            return Ok();
        }
        [HttpGet("categories")]
        public async Task<ActionResult<IEnumerable<ServiceCategoryDto>>> GetServiceCategories()
        {
            var result = await _servicesService.GetServiceCategories();
            return Ok(result.Data);
        }
        
        [HttpDelete("{id}")]
        [Authorize(Policy = "AdminAccessToken")]
        public async Task<ActionResult<Part>> DeleteService(short id)
        {
            var result = await _servicesService.DeleteService(id);
            if (result.Status == ServiceStatus.NotFound)
            {
                return NotFound(result.Message);
            }
            return Ok();
        }
    }
}
