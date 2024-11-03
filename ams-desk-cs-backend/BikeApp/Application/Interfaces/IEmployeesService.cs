using ams_desk_cs_backend.BikeApp.Application.Results;
using ams_desk_cs_backend.BikeApp.Dtos.AppModelDto;

namespace ams_desk_cs_backend.BikeApp.Application.Interfaces
{
    public interface IEmployeesService
    {
        public Task<ServiceResult<IEnumerable<EmployeeDto>>> GetEmployees();
    }
}
