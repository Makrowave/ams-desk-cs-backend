using ams_desk_cs_backend.BikeApp.Dtos.Repairs;
using ams_desk_cs_backend.Shared.Results;

namespace ams_desk_cs_backend.BikeApp.Interfaces
{
    public interface IPartTypesService
    {
        public Task<ServiceResult<IEnumerable<PartTypeDto>>> GetPartTypes(short id);
        public Task<ServiceResult<IEnumerable<PartCategoryDto>>> GetPartCategories();
    }
}
