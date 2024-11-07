using ams_desk_cs_backend.BikeApp.Dtos.AppModelDto;
using ams_desk_cs_backend.Shared.Results;

namespace ams_desk_cs_backend.BikeApp.Application.Interfaces
{
    public interface IStatusService
    {
        public Task<ServiceResult<IEnumerable<StatusDto>>> GetStatuses();
        public Task<ServiceResult<IEnumerable<StatusDto>>> GetStatusesNotSold();
        public Task<ServiceResult<StatusDto>> GetStatus(short id);
    }
}
