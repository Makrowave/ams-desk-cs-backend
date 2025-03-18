using ams_desk_cs_backend.BikeApp.Dtos.AppModelDto;
using ams_desk_cs_backend.Shared.Results;

namespace ams_desk_cs_backend.BikeApp.Interfaces
{
    public interface IColorsService
    {
        public Task<ServiceResult<IEnumerable<ColorDto>>> GetColors();
        public Task<ServiceResult<ColorDto>> GetColor(short id);
        public Task<ServiceResult<ColorDto>> PostColor(ColorDto color);
        public Task<ServiceResult<ColorDto>> UpdateColor(short id, ColorDto color);
        public Task<ServiceResult> ChangeOrder(short firstId, short lastId);
        public Task<ServiceResult> DeleteColor(short id);

    }
}
