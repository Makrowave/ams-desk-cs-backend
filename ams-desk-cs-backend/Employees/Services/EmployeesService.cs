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
                Id = employee.EmployeeId,
                Name = employee.EmployeeName,
            }).ToListAsync();
        return new ServiceResult<IEnumerable<EmployeeDto>>(ServiceStatus.Ok, string.Empty, employees);
    }

    public async Task<ServiceResult<EmployeeDto>> PostEmployee(EmployeeDto employeeDto)
    {
        var order = _context.Employees.Count() + 1;
        var employee = new Employee
        {
            EmployeeName = employeeDto.Name,
            EmployeesOrder = (short)order,
        };
        _context.Add(employee);
        await _context.SaveChangesAsync();
        var result = new EmployeeDto
        {
            Id = employee.EmployeeId,
            Name = employee.EmployeeName,
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

        existingEmployee.EmployeeName = employee.Name;
        await _context.SaveChangesAsync();
        var result = new EmployeeDto
        {
            Id = existingEmployee.EmployeeId,
            Name = existingEmployee.EmployeeName,
        };
        return new ServiceResult<EmployeeDto>(ServiceStatus.Ok, string.Empty, result);
    }

    public async Task<ServiceResult<List<EmployeeDto>>> ChangeOrder(short source, short dest)
    {
        if (!_context.Employees.Any(e => e.EmployeeId == source || e.EmployeeId == dest))
        {
            return ServiceResult<List<EmployeeDto>>.NotFound("Nie znaleziono zamienianych pracowników");
        }

        var employees = await _context.Employees.OrderBy(e => e.EmployeesOrder).ToListAsync();
        var sourceEmployee = employees.First(e => e.EmployeeId == source);
        var destEmployee = employees.First(e => e.EmployeeId == dest);

        var sourceOrder = sourceEmployee.EmployeesOrder;
        var destOrder = destEmployee.EmployeesOrder;

        if (sourceOrder < destOrder)
        {
            employees
                .Where(e => e.EmployeesOrder > sourceOrder && e.EmployeesOrder <= destOrder)
                .ToList()
                .ForEach(e => e.EmployeesOrder--);

            sourceEmployee.EmployeesOrder = destOrder;
        }
        else if (sourceOrder > destOrder)
        {
            employees
                .Where(e => e.EmployeesOrder >= destOrder && e.EmployeesOrder < sourceOrder)
                .ToList()
                .ForEach(e => e.EmployeesOrder++);

            sourceEmployee.EmployeesOrder = destOrder;
        }

        await _context.SaveChangesAsync();

        var result = await _context.Employees
            .OrderBy(e => e.EmployeesOrder)
            .Select(e => new EmployeeDto
            {
                Id = e.EmployeeId,
                Name = e.EmployeeName
            })
            .ToListAsync();

        return new ServiceResult<List<EmployeeDto>>(ServiceStatus.Ok, string.Empty, result);
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