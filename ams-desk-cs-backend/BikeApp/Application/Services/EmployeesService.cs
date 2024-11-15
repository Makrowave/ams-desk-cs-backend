using ams_desk_cs_backend.BikeApp.Application.Interfaces;
using ams_desk_cs_backend.BikeApp.Dtos.AppModelDto;
using ams_desk_cs_backend.BikeApp.Infrastructure.Data;
using ams_desk_cs_backend.Shared.Results;
using Microsoft.EntityFrameworkCore;

namespace ams_desk_cs_backend.BikeApp.Application.Services
{
    public class EmployeesService : IEmployeesService
    {
        private readonly BikesDbContext _context;
        public EmployeesService(BikesDbContext context)
        {
            _context = context;
        }
        public async Task<ServiceResult<IEnumerable<EmployeeDto>>> GetEmployees()
        {
            var employees = await _context.Employees.Select(employee => new EmployeeDto
            {
                EmployeeId = employee.EmployeeId,
                EmployeeName = employee.EmployeeName,
            }).OrderBy(employee => employee.EmployeeId).ToListAsync();
            return new ServiceResult<IEnumerable<EmployeeDto>>(ServiceStatus.Ok, string.Empty, employees);
        }
    }
}
