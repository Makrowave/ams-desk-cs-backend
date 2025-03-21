using ams_desk_cs_backend.BikeApp.Dtos.Repairs;
using ams_desk_cs_backend.Shared.Results;

namespace ams_desk_cs_backend.BikeApp.Interfaces
{
    public interface IRepairsService
    {
        public Task<ServiceResult<IEnumerable<ShortRepairDto>>> GetRepairs(short place, short[] excludedStatuses);
        public Task<ServiceResult<RepairDto>> GetRepair(int id);
        public Task<ServiceResult<int>> CreateRepair(NewRepairDto newRepair);
        public Task<ServiceResult<RepairDto>> UpdateRepair(int id, RepairDto newRepair);
        public Task<ServiceResult<RepairDto>> UpdateStatus(int id, short statusId);
        public Task<ServiceResult<RepairDto>> UpdateEmployee(int id, short employeeId, bool collection);
        public Task<ServiceResult<RepairDto>> StartRepair(int id, short employeeId);
        public Task<ServiceResult<RepairDto>> CollectRepair(int id, short employeeId);
    }
}
