using ams_desk_cs_backend.BikeApp.Dtos.Repairs;
using ams_desk_cs_backend.Shared.Results;

namespace ams_desk_cs_backend.BikeApp.Interfaces
{
    public interface IPartCategoriesService
    {
        public abstract Task<ServiceResult<IEnumerable<PartCategoryDto>>> GetPartCategories();
    }
}
