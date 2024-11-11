using ams_desk_cs_backend.BikeApp.Dtos.AppModelDto;
using ams_desk_cs_backend.Shared.Results;

namespace ams_desk_cs_backend.BikeApp.Application.Interfaces
{
    public interface IColorsService
    {
        Task<ServiceResult<IEnumerable<ColorDto>>> GetColors();
        Task<ServiceResult<ColorDto>> GetColor(short id);
        Task<ServiceResult> PostColor(ColorDto color);
        Task<ServiceResult> UpdateColor(short id, ColorDto color);
        Task<ServiceResult> DeleteColor(short id);

    }
}
