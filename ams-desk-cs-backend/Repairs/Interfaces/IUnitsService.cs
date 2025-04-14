using ams_desk_cs_backend.Repairs.Dtos;
using ams_desk_cs_backend.Shared.Results;

namespace ams_desk_cs_backend.Repairs.Interfaces;

public interface IUnitsService
{
    public abstract Task<ServiceResult<IEnumerable<UnitDto>>> GetUnits();
}