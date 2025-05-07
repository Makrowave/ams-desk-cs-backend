using ams_desk_cs_backend.Employees.Dtos;
using ams_desk_cs_backend.Shared.Results;

namespace ams_desk_cs_backend.Employees.Interfaces;

public interface IEmployeesService
{
    public Task<ServiceResult<IEnumerable<EmployeeDto>>> GetEmployees();
    public Task<ServiceResult<EmployeeDto>> PostEmployee(EmployeeDto employee);
    public Task<ServiceResult<EmployeeDto>> UpdateEmployee(short id, EmployeeDto employee);
    public Task<ServiceResult<List<EmployeeDto>>> ChangeOrder(short firstId, short lastId);
    public Task<ServiceResult> DeleteEmployee(short id);
}