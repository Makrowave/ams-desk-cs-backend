using ams_desk_cs_backend.BikeApp.Application.Results;
using ams_desk_cs_backend.BikeApp.Dtos.AppModelDto;
using ams_desk_cs_backend.BikeApp.Infrastructure.Data.Models;

namespace ams_desk_cs_backend.BikeApp.Application.Interfaces
{
    public interface ICategoriesService
    {
        public Task<ServiceResult<IEnumerable<CategoryDto>>> GetCategories();
    }
}
