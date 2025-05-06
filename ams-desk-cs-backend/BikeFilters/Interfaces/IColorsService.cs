using ams_desk_cs_backend.BikeFilters.Dtos;
using ams_desk_cs_backend.Shared.Results;

namespace ams_desk_cs_backend.BikeFilters.Interfaces;

public interface IColorsService
{
    public Task<ServiceResult<IEnumerable<ColorDto>>> GetColors();
    public Task<ServiceResult<ColorDto>> GetColor(short id);
    public Task<ServiceResult<ColorDto>> PostColor(ColorDto color);
    public Task<ServiceResult<ColorDto>> UpdateColor(short id, ColorDto color);
    public Task<ServiceResult<List<ColorDto>>> ChangeOrder(short source, short dest);
    public Task<ServiceResult> DeleteColor(short id);

}