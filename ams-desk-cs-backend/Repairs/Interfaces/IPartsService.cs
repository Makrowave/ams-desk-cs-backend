using ams_desk_cs_backend.Data.Models.Repairs;
using ams_desk_cs_backend.Shared.Results;

namespace ams_desk_cs_backend.Repairs.Interfaces;

public interface IPartsService
{
    public Task<ServiceResult<IEnumerable<Part>>> GetParts();
    public Task<ServiceResult<IEnumerable<Part>>> GetFilteredParts(short categoryId, short typeId);
    public Task<ServiceResult<Part>> AddPart(Part part);
    public Task<ServiceResult<Part>> ChangePart(int partId, Part part);
    public Task<ServiceResult> DeletePart(int id);
}