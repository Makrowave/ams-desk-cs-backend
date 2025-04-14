using ams_desk_cs_backend.Repairs.Dtos;
using ams_desk_cs_backend.Shared.Results;

namespace ams_desk_cs_backend.Repairs.Interfaces;

public interface IPartTypesService
{
    public Task<ServiceResult<IEnumerable<PartTypeDto>>> GetPartTypes(short id);
    public Task<ServiceResult<IEnumerable<PartCategoryDto>>> GetPartCategories();
}