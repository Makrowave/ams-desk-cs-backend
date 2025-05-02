using ams_desk_cs_backend.Bikes.Dtos;
using ams_desk_cs_backend.Shared.Results;

namespace ams_desk_cs_backend.Bikes.Interfaces;

public interface IBikesService
{
    Task<ServiceResult<BikeSubRecordDto>> PutBike(int id, BikeDto bike);
    Task<ServiceResult<IEnumerable<BikeSubRecordDto>>> GetBikes(int modelId, short PlaceId);
    Task<ServiceResult<BikeSubRecordDto>> PostBike(BikeDto bike);
        
    Task<ServiceResult<(short PlaceId, int ModelId)>> SellBike(int id, int price, bool internet);
    Task<ServiceResult> DeleteBike(int id);
    Task<ServiceResult<BikeSubRecordDto>> MoveBike(int id, short placeId);
    Task<ServiceResult<BikeSubRecordDto>> AssembleBikeMobile(int id, short employeeId);
}