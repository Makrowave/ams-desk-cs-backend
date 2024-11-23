using ams_desk_cs_backend.BikeApp.Dtos.AppModelDto;
using ams_desk_cs_backend.Shared.Results;

namespace ams_desk_cs_backend.BikeApp.Application.Interfaces
{
    public interface IEmployeesService
    {
        public Task<ServiceResult<IEnumerable<EmployeeDto>>> GetEmployees();
        public Task<ServiceResult> PostEmployee(EmployeeDto employee);
        public Task<ServiceResult> UpdateEmployee(short id, EmployeeDto employee);
        public Task<ServiceResult> ChangeOrder(short firstId, short lastId);
        public Task<ServiceResult> DeleteEmployee(short id);
    }
}
