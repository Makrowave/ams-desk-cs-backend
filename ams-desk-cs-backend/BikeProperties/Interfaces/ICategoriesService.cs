using ams_desk_cs_backend.BikeFilters.Dtos;
using ams_desk_cs_backend.Shared.Results;

namespace ams_desk_cs_backend.BikeFilters.Interfaces;

public interface ICategoriesService
{
    public Task<ServiceResult<IEnumerable<CategoryDto>>> GetCategories();
    public Task<ServiceResult<CategoryDto>> PostCategory(CategoryDto category);
    public Task<ServiceResult<CategoryDto>> UpdateCategory(short id, CategoryDto category);
    public Task<ServiceResult<List<CategoryDto>>> ChangeOrder(short source, short dest);
    public Task<ServiceResult> DeleteCategory(short id);

}