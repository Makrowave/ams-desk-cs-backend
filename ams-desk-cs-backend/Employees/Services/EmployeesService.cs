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
        var employees = await _context.Employees.OrderBy(employee => employee.Order)
            .Select(employee => new EmployeeDto
            {
                Id = employee.iD,
                Name = employee.Name,
            }).ToListAsync();
        return new ServiceResult<IEnumerable<EmployeeDto>>(ServiceStatus.Ok, string.Empty, employees);
    }

    public async Task<ServiceResult<EmployeeDto>> PostEmployee(EmployeeDto employeeDto)
    {
        var order = _context.Employees.Count() + 1;
        var employee = new Employee
        {
            Name = employeeDto.Name,
            Order = (short)order,
        };
        _context.Add(employee);
        await _context.SaveChangesAsync();
        var result = new EmployeeDto
        {
            Id = employee.iD,
            Name = employee.Name,
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

        existingEmployee.Name = employee.Name;
        await _context.SaveChangesAsync();
        var result = new EmployeeDto
        {
            Id = existingEmployee.iD,
            Name = existingEmployee.Name,
        };
        return new ServiceResult<EmployeeDto>(ServiceStatus.Ok, string.Empty, result);
    }

    public async Task<ServiceResult<List<EmployeeDto>>> ChangeOrder(short source, short dest)
    {
        if (!_context.Employees.Any(e => e.iD == source || e.iD == dest))
        {
            return ServiceResult<List<EmployeeDto>>.NotFound("Nie znaleziono zamienianych pracowników");
        }

        var employees = await _context.Employees.OrderBy(e => e.Order).ToListAsync();
        var sourceEmployee = employees.First(e => e.iD == source);
        var destEmployee = employees.First(e => e.iD == dest);

        var sourceOrder = sourceEmployee.Order;
        var destOrder = destEmployee.Order;

        if (sourceOrder < destOrder)
        {
            employees
                .Where(e => e.Order > sourceOrder && e.Order <= destOrder)
                .ToList()
                .ForEach(e => e.Order--);

            sourceEmployee.Order = destOrder;
        }
        else if (sourceOrder > destOrder)
        {
            employees
                .Where(e => e.Order >= destOrder && e.Order < sourceOrder)
                .ToList()
                .ForEach(e => e.Order++);

            sourceEmployee.Order = destOrder;
        }

        await _context.SaveChangesAsync();

        var result = await _context.Employees
            .OrderBy(e => e.Order)
            .Select(e => new EmployeeDto
            {
                Id = e.iD,
                Name = e.Name
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