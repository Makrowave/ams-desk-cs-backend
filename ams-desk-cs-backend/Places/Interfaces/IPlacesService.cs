using ams_desk_cs_backend.Places.Dtos;
using ams_desk_cs_backend.Shared.Results;

namespace ams_desk_cs_backend.Places.Interfaces
{
    public interface IPlacesService
    {
        public Task<ServiceResult<IEnumerable<PlaceDto>>> GetPlaces();
        public Task<ServiceResult> ChangeOrder(short firstId, short lastId);
    }
}
