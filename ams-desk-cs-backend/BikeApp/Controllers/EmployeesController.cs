using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ams_desk_cs_backend.BikeApp.Dtos.AppModelDto;
using ams_desk_cs_backend.Shared.Results;
using ams_desk_cs_backend.BikeApp.Interfaces;

namespace ams_desk_cs_backend.BikeApp.Controllers
{
    [Authorize(Policy = "AccessToken")]
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeesController : ControllerBase
    {
        private readonly IEmployeesService _employeesService;

        public EmployeesController(IEmployeesService employeesService)
        {
            _employeesService = employeesService;
        }

        // GET: api/Employees
        [HttpGet]
        public async Task<ActionResult<IEnumerable<EmployeeDto>>> GetEmployees()
        {
            var result = await _employeesService.GetEmployees();
            return Ok(result.Data);
        }
        [HttpPost]
        [Authorize(Policy = "AdminAccessToken")]
        public async Task<ActionResult<EmployeeDto>> AddEmployee(EmployeeDto employee)
        {
            var result = await _employeesService.PostEmployee(employee);
            if (result.Status == ServiceStatus.BadRequest)
            {
                return NotFound(result.Message);
            }
            return Ok(result.Data);
        }
        [HttpPut("{id}")]
        [Authorize(Policy = "AdminAccessToken")]
        public async Task<ActionResult<EmployeeDto>> UpdateEmployee(short id, EmployeeDto employee)
        {
            var result = await _employeesService.UpdateEmployee(id, employee);
            if (result.Status == ServiceStatus.NotFound)
            {
                return NotFound(result.Message);
            }
            return Ok(result.Data);
        }
        [HttpPut("ChangeOrder")]
        [Authorize(Policy = "AdminAccessToken")]
        public async Task<IActionResult> ChangeOrder(short first, short last)
        {
            var result = await _employeesService.ChangeOrder(first, last);
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
        public async Task<IActionResult> DeleteEmployee(short id)
        {
            var result = await _employeesService.DeleteEmployee(id);
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
