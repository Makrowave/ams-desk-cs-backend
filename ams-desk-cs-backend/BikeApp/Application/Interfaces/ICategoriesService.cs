using ams_desk_cs_backend.BikeApp.Dtos.AppModelDto;
using ams_desk_cs_backend.Shared.Results;

namespace ams_desk_cs_backend.BikeApp.Application.Interfaces
{
    public interface ICategoriesService
    {
        public Task<ServiceResult<IEnumerable<CategoryDto>>> GetCategories();
        public Task<ServiceResult> PostCategory(CategoryDto category);
        public Task<ServiceResult> UpdateCategory(short id, CategoryDto category);
        public Task<ServiceResult> DeleteCategory(short id);

    }
}
