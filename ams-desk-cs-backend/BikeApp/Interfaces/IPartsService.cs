using ams_desk_cs_backend.BikeApp.Data.Models.Repairs;
using ams_desk_cs_backend.Shared.Results;

namespace ams_desk_cs_backend.BikeApp.Interfaces
{
    public interface IPartsService
    {
        public Task<ServiceResult<IEnumerable<Part>>> GetParts();
        public Task<ServiceResult> AddPart(Part part);
        public Task<ServiceResult<Part>> ChangePart(int partId, Part part);
        public Task<ServiceResult> DeletePart(int id);
    }
}
