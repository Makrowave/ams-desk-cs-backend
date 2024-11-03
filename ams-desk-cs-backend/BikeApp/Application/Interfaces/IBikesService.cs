using ams_desk_cs_backend.BikeApp.Api.Dtos;
using ams_desk_cs_backend.BikeApp.Application.Results;
using ams_desk_cs_backend.BikeApp.Dtos.AppModelDto;
namespace ams_desk_cs_backend.BikeApp.Application.Interfaces
{
    public interface IBikesService
    {
        Task<ServiceResult> PutBike(int id, BikeDto bike);
        Task<ServiceResult<IEnumerable<BikeSubRecordDto>>> GetBikes(int modelId, short PlaceId);
        Task<ServiceResult> PostBike(BikeDto bike);
        Task<ServiceResult> DeleteBike(int id);
    }
}
