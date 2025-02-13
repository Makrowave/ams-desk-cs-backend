using ams_desk_cs_backend.BikeApp.Dtos.AppModelDto;
using ams_desk_cs_backend.Shared.Results;

namespace ams_desk_cs_backend.BikeApp.Interfaces
{
    public interface IStatusService
    {
        public Task<ServiceResult<IEnumerable<StatusDto>>> GetStatuses();
        public Task<ServiceResult<IEnumerable<StatusDto>>> GetStatusesExcluded(int[] excludedStatuses);
        public Task<ServiceResult<StatusDto>> GetStatus(short id);
        public Task<ServiceResult> PostStatus(StatusDto color);
        public Task<ServiceResult> UpdateStatus(short id, StatusDto color);
        public Task<ServiceResult> ChangeOrder(short firstId, short lastId);
        public Task<ServiceResult> DeleteStatus(short id);
    }
}
