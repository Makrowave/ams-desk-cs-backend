using ams_desk_cs_backend.BikeApp.Data;
using ams_desk_cs_backend.BikeApp.Dtos.AppModelDto;
using ams_desk_cs_backend.BikeApp.Data.Models;
using ams_desk_cs_backend.BikeApp.Interfaces;
using ams_desk_cs_backend.Shared.Results;
using Microsoft.EntityFrameworkCore;

namespace ams_desk_cs_backend.BikeApp.Services
{
    public class StatusService : IStatusService
    {
        private readonly BikesDbContext _context;
        public StatusService(BikesDbContext context)
        {
            _context = context;
        }


        public async Task<ServiceResult<StatusDto>> GetStatus(short id)
        {
            var status = await _context.Statuses.FindAsync(id);
            if (status == null)
            {
                return new ServiceResult<StatusDto>(ServiceStatus.NotFound, "Nie znaleziono statusu", null);
            }
            return new ServiceResult<StatusDto>(ServiceStatus.Ok, string.Empty, new StatusDto
            {
                StatusId = status.StatusId,
                StatusName = status.StatusName,
                HexCode = status.HexCode,
            });
        }

        public async Task<ServiceResult<IEnumerable<StatusDto>>> GetStatuses()
        {
            var statuses = await _context.Statuses.OrderBy(status => status.StatusesOrder)
                .Select(status => new StatusDto
                {
                    StatusId = status.StatusId,
                    StatusName = status.StatusName,
                    HexCode = status.HexCode,
                }).ToListAsync();
            return new ServiceResult<IEnumerable<StatusDto>>(ServiceStatus.Ok, string.Empty, statuses);
        }

        public async Task<ServiceResult<IEnumerable<StatusDto>>> GetStatusesExcluded(int[] excludedStatuses)
        {
            var statuses = await _context.Statuses
                .Where(status => !excludedStatuses.Any(s => s == status.StatusId))
                .OrderBy(status => status.StatusesOrder)
                .Select(status => new StatusDto
                {
                    StatusId = status.StatusId,
                    StatusName = status.StatusName,
                    HexCode = status.HexCode,
                }).OrderBy(status => status.StatusId).ToListAsync();
            return new ServiceResult<IEnumerable<StatusDto>>(ServiceStatus.Ok, string.Empty, statuses);
        }
        public async Task<ServiceResult> ChangeOrder(short firstId, short lastId)
        {
            if (!_context.Statuses.Any(status => status.StatusId == firstId || status.StatusId == lastId))
            {
                return new ServiceResult(ServiceStatus.NotFound, "Nie znaleziono zamienianych elementów");
            }
            var statuses = await _context.Statuses.OrderBy(status => status.StatusesOrder).ToListAsync();
            var firstOrder = statuses.FirstOrDefault(status => status.StatusId == firstId)!.StatusesOrder;
            var lastOrder = statuses.FirstOrDefault(status => status.StatusId == lastId)!.StatusesOrder;

            var filteredStatuses = statuses.Where(status => status.StatusesOrder >= firstOrder && status.StatusesOrder <= lastOrder).ToList();
            filteredStatuses.ForEach(status => status.StatusesOrder++);
            filteredStatuses.Last().StatusesOrder = firstOrder;
            await _context.SaveChangesAsync();
            return new ServiceResult(ServiceStatus.Ok, string.Empty);
        }

        public async Task<ServiceResult> PostStatus(StatusDto status)
        {
            var order = _context.Statuses.Count() + 1;
            _context.Add(new Status
            {
                StatusName = status.StatusName,
                HexCode = status.HexCode,
                StatusesOrder = (short)order
            });
            await _context.SaveChangesAsync();
            return new ServiceResult(ServiceStatus.Ok, string.Empty);
        }

        public async Task<ServiceResult> UpdateStatus(short id, StatusDto newStatus)
        {
            var oldStatus = await _context.Statuses.FindAsync(id);
            if (oldStatus == null)
            {
                return new ServiceResult(ServiceStatus.NotFound, "Nie znaleziono statusu");
            }

            oldStatus.StatusName = newStatus.StatusName;
            oldStatus.HexCode = newStatus.HexCode;
            await _context.SaveChangesAsync();
            return new ServiceResult(ServiceStatus.Ok, string.Empty);
        }

        public async Task<ServiceResult> DeleteStatus(short id)
        {
            try
            {
                var existingStatus = await _context.Statuses.FindAsync(id);
                if (existingStatus == null)
                {
                    return new ServiceResult(ServiceStatus.NotFound, "Nie znaleziono statusu");
                }
                _context.Statuses.Remove(existingStatus);
                await _context.SaveChangesAsync();
                return new ServiceResult(ServiceStatus.Ok, string.Empty);
            }
            catch (Exception ex)
            {
                return new ServiceResult(ServiceStatus.BadRequest, "Status jest przypisany do roweru");
            }
        }
    }
}
