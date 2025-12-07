using ams_desk_cs_backend.BikeProperties.Dtos;
using ams_desk_cs_backend.Shared.Results;

namespace ams_desk_cs_backend.BikeProperties.Interfaces;

public interface IManufacturersService
{
    public Task<ServiceResult<IEnumerable<ManufacturerDto>>> GetManufacturers();
    public Task<ServiceResult<ManufacturerDto>> PostManufacturer(ManufacturerDto manufacturer);
    public Task<ServiceResult<ManufacturerDto>> UpdateManufacturer(short id, ManufacturerDto manufacturer);
    public Task<ServiceResult<List<ManufacturerDto>>> ChangeOrder(short source, short dest);
    public Task<ServiceResult> DeleteManufacturer(short id);
}