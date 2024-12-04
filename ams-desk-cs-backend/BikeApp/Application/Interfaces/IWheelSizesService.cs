using ams_desk_cs_backend.BikeApp.Api.Dtos;
using ams_desk_cs_backend.Shared.Results;

namespace ams_desk_cs_backend.BikeApp.Application.Interfaces;

public interface IWheelSizesService
{
    public Task<ServiceResult<IEnumerable<WheelSizeDto>>> GetWheelSizes();
    public Task<ServiceResult> PostWheelSize(short wheelSize);
    public Task<ServiceResult> DeleteWheelSize(short wheelSize);
}