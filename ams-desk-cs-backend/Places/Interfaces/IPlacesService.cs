using ams_desk_cs_backend.Places.Dtos;
using ams_desk_cs_backend.Shared.Results;

namespace ams_desk_cs_backend.Places.Interfaces
{
    public interface IPlacesService
    {
        public Task<ServiceResult<IEnumerable<PlaceDto>>> GetPlaces();
        public Task<ServiceResult<IEnumerable<PlaceDto>>> GetPlacesNotStorage();
        public Task<ServiceResult<List<PlaceDto>>> ChangeOrder(short firstId, short lastId);
        public Task<ServiceResult<PlaceDto>> PostPlace(PlaceDto placeDto);
        public Task<ServiceResult<PlaceDto>> PutPlace(short id, PlaceDto placeDto);
        public Task<ServiceResult> DeletePlace(short id);
    }
}
