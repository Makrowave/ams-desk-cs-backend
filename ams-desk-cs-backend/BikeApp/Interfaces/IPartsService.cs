using ams_desk_cs_backend.BikeApp.Data.Models.Repairs;
using ams_desk_cs_backend.Shared.Results;

namespace ams_desk_cs_backend.BikeApp.Interfaces
{
    public interface IPartsService
    {
        public abstract Task<ServiceResult<IEnumerable<Part>>> GetParts();
    }
}
