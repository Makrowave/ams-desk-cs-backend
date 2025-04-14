using ams_desk_cs_backend.Data;
using ams_desk_cs_backend.Data.Models;
using ams_desk_cs_backend.Employees.Dtos;
using ams_desk_cs_backend.Employees.Interfaces;
using ams_desk_cs_backend.Shared.Results;
using Microsoft.EntityFrameworkCore;

namespace ams_desk_cs_backend.Employees.Services;

public class EmployeesService : IEmployeesService
{
    private readonly BikesDbContext _context;

    public EmployeesService(BikesDbContext context)
    {
        _context = context;
    }

    public async Task<ServiceResult<IEnumerable<EmployeeDto>>> GetEmployees()
    {
        var employees = await _context.Employees.OrderBy(employee => employee.EmployeesOrder)
            .Select(employee => new EmployeeDto
            {
                EmployeeId = employee.EmployeeId,
                EmployeeName = employee.EmployeeName,
            }).ToListAsync();
        return new ServiceResult<IEnumerable<EmployeeDto>>(ServiceStatus.Ok, string.Empty, employees);
    }

    public async Task<ServiceResult<EmployeeDto>> PostEmployee(EmployeeDto employeeDto)
    {
        var order = _context.Employees.Count() + 1;
        var employee = new Employee
        {
            EmployeeName = employeeDto.EmployeeName,
            EmployeesOrder = (short)order,
        };
        _context.Add(employee);
        await _context.SaveChangesAsync();
        var result = new EmployeeDto
        {
            EmployeeId = employee.EmployeeId,
            EmployeeName = employee.EmployeeName,
        };
        return new ServiceResult<EmployeeDto>(ServiceStatus.Ok, string.Empty, result);
    }

    public async Task<ServiceResult<EmployeeDto>> UpdateEmployee(short id, EmployeeDto employee)
    {
        var existingEmployee = await _context.Employees.FindAsync(id);
        if (existingEmployee == null)
        {
            return ServiceResult<EmployeeDto>.NotFound("Nie znaleziono pracownika");
        }

        existingEmployee.EmployeeName = employee.EmployeeName;
        await _context.SaveChangesAsync();
        var result = new EmployeeDto
        {
            EmployeeId = existingEmployee.EmployeeId,
            EmployeeName = existingEmployee.EmployeeName,
        };
        return new ServiceResult<EmployeeDto>(ServiceStatus.Ok, string.Empty, result);
    }

    public async Task<ServiceResult> ChangeOrder(short firstId, short lastId)
    {
        if (!_context.Employees.Any(employee => employee.EmployeeId == firstId || employee.EmployeeId == lastId))
        {
            return new ServiceResult(ServiceStatus.NotFound, "Nie znaleziono zamienianych elementów");
        }

        var employees = await _context.Employees.OrderBy(employee => employee.EmployeesOrder).ToListAsync();
        var firstOrder = employees.FirstOrDefault(employee => employee.EmployeeId == firstId)!.EmployeesOrder;
        var lastOrder = employees.FirstOrDefault(employee => employee.EmployeeId == lastId)!.EmployeesOrder;

        var filteredEmployees = employees.Where(employee =>
            employee.EmployeesOrder >= firstOrder && employee.EmployeesOrder <= lastOrder).ToList();
        filteredEmployees.ForEach(employee => employee.EmployeesOrder++);
        filteredEmployees.Last().EmployeesOrder = firstOrder;
        await _context.SaveChangesAsync();
        return new ServiceResult(ServiceStatus.Ok, string.Empty);
    }

    public async Task<ServiceResult> DeleteEmployee(short id)
    {
        try
        {
            var existingEmployee = await _context.Employees.FindAsync(id);
            if (existingEmployee == null)
            {
                return new ServiceResult(ServiceStatus.NotFound, "Nie znaleziono pracownika");
            }

            _context.Employees.Remove(existingEmployee);
            await _context.SaveChangesAsync();
            return new ServiceResult(ServiceStatus.Ok, string.Empty);
        }
        catch (Exception ex)
        {
            return new ServiceResult(ServiceStatus.BadRequest, "Pracownik jest przypisany do rowerów");
        }
    }
}