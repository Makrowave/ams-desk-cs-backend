using ams_desk_cs_backend.BikeApp.Dtos.Repairs;
using ams_desk_cs_backend.Shared.Results;

namespace ams_desk_cs_backend.BikeApp.Interfaces
{
    public interface IUnitsService
    {
        public abstract Task<ServiceResult<IEnumerable<UnitDto>>> GetUnits();
    }
}
