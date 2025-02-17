using ams_desk_cs_backend.BikeApp.Dtos.Repairs;
using ams_desk_cs_backend.Shared.Results;

namespace ams_desk_cs_backend.BikeApp.Interfaces
{
    public interface IRepairsService
    {
        public Task<ServiceResult<IEnumerable<ShortRepairDto>>> GetRepairs(short place, short[] excludedStatuses);
        public Task<ServiceResult<RepairDto>> GetRepair(int id);
        public Task<ServiceResult<int>> CreateRepair(NewRepairDto newRepair);
        public Task<ServiceResult> UpdateRepair(int id, RepairDto newRepair);
    }
}
