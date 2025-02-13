using ams_desk_cs_backend.BikeApp.Dtos;
using ams_desk_cs_backend.BikeApp.Dtos.AppModelDto;
using ams_desk_cs_backend.Shared.Results;
namespace ams_desk_cs_backend.BikeApp.Interfaces
{
    public interface IBikesService
    {
        Task<ServiceResult> PutBike(int id, BikeDto bike);
        Task<ServiceResult<IEnumerable<BikeSubRecordDto>>> GetBikes(int modelId, short PlaceId);
        Task<ServiceResult> PostBike(BikeDto bike);
        Task<ServiceResult> DeleteBike(int id);
    }
}
