using ams_desk_cs_backend.BikeFilters.Dtos;
using ams_desk_cs_backend.Shared.Results;

namespace ams_desk_cs_backend.BikeFilters.Interfaces;

public interface IWheelSizesService
{
    public Task<ServiceResult<IEnumerable<WheelSizeDto>>> GetWheelSizes();
    public Task<ServiceResult> PostWheelSize(short wheelSize);
    public Task<ServiceResult> DeleteWheelSize(short wheelSize);
}