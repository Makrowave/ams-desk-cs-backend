using ams_desk_cs_backend.BikeApp.Application.Results;
using ams_desk_cs_backend.BikeApp.Dtos.AppModelDto;

namespace ams_desk_cs_backend.BikeApp.Application.Interfaces
{
    public interface IManufacturersService
    {
        public Task<ServiceResult<IEnumerable<ManufacturerDto>>> GetManufacturers();
    }
}
