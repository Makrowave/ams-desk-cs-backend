﻿using ams_desk_cs_backend.BikeFilters.Dtos;
using ams_desk_cs_backend.Shared.Results;

namespace ams_desk_cs_backend.BikeFilters.Interfaces;

public interface IStatusService
{
    public Task<ServiceResult<IEnumerable<StatusDto>>> GetStatuses();
    public Task<ServiceResult<IEnumerable<StatusDto>>> GetStatusesExcluded(int[] excludedStatuses);
    public Task<ServiceResult<StatusDto>> GetStatus(short id);
    public Task<ServiceResult<StatusDto>> PostStatus(StatusDto color);
    public Task<ServiceResult<StatusDto>> UpdateStatus(short id, StatusDto color);
    public Task<ServiceResult<List<StatusDto>>> ChangeOrder(short source, short dest);
    public Task<ServiceResult> DeleteStatus(short id);
}