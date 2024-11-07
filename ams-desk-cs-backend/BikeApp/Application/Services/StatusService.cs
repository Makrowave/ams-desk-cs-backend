using ams_desk_cs_backend.BikeApp.Application.Interfaces;
using ams_desk_cs_backend.BikeApp.Dtos.AppModelDto;
using ams_desk_cs_backend.BikeApp.Infrastructure.Data;
using ams_desk_cs_backend.BikeApp.Infrastructure.Enums;
using ams_desk_cs_backend.Shared.Results;
using Microsoft.EntityFrameworkCore;

namespace ams_desk_cs_backend.BikeApp.Application.Services
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
            if(status == null)
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
            var statuses = await _context.Statuses.Select(status => new StatusDto
            {
                StatusId = status.StatusId,
                StatusName = status.StatusName,
                HexCode = status.HexCode,
            }).ToListAsync();
            return new ServiceResult<IEnumerable<StatusDto>>(ServiceStatus.Ok, string.Empty, statuses);
        }

        public async Task<ServiceResult<IEnumerable<StatusDto>>> GetStatusesNotSold()
        {
            var statuses = await _context.Statuses
                .Where(status => status.StatusId != (short)BikeStatus.Sold)
                .Select(status => new StatusDto
                {
                    StatusId = status.StatusId,
                    StatusName = status.StatusName,
                    HexCode = status.HexCode,
                }).ToListAsync();
            return new ServiceResult<IEnumerable<StatusDto>>(ServiceStatus.Ok, string.Empty, statuses);
        }
    }
}
