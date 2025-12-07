using ams_desk_cs_backend.BikeProperties.Dtos;
using ams_desk_cs_backend.Shared.Results;

namespace ams_desk_cs_backend.BikeProperties.Interfaces;

public interface IWheelSizesService
{
    public Task<ServiceResult<IEnumerable<WheelSizeDto>>> GetWheelSizes();
    public Task<ServiceResult<WheelSizeDto>> PostWheelSize(decimal wheelSize);
    public Task<ServiceResult> DeleteWheelSize(int wheelSize);
}