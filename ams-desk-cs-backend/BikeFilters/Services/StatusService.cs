using ams_desk_cs_backend.BikeFilters.Dtos;
using ams_desk_cs_backend.BikeFilters.Interfaces;
using ams_desk_cs_backend.Data;
using ams_desk_cs_backend.Data.Models;
using ams_desk_cs_backend.Shared.Results;
using Microsoft.EntityFrameworkCore;

namespace ams_desk_cs_backend.BikeFilters.Services;

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
            .Where(status => excludedStatuses.All(s => s != status.StatusId))
            .OrderBy(status => status.StatusesOrder)
            .Select(status => new StatusDto
            {
                StatusId = status.StatusId,
                StatusName = status.StatusName,
                HexCode = status.HexCode,
            }).OrderBy(status => status.StatusId).ToListAsync();
        return new ServiceResult<IEnumerable<StatusDto>>(ServiceStatus.Ok, string.Empty, statuses);
    }
    public async Task<ServiceResult<List<StatusDto>>> ChangeOrder(short source, short dest)
    {
        if (!_context.Statuses.Any(s => s.StatusId == source || s.StatusId == dest))
        {
            return ServiceResult<List<StatusDto>>.NotFound("Nie znaleziono zamienianych elementów");
        }

        var statuses = await _context.Statuses.OrderBy(s => s.StatusesOrder).ToListAsync();
        var sourceStatus = statuses.First(s => s.StatusId == source);
        var destStatus = statuses.First(s => s.StatusId == dest);

        var sourceOrder = sourceStatus.StatusesOrder;
        var destOrder = destStatus.StatusesOrder;

        if (sourceOrder < destOrder)
        {
            statuses
                .Where(s => s.StatusesOrder > sourceOrder && s.StatusesOrder <= destOrder)
                .ToList()
                .ForEach(s => s.StatusesOrder--);

            sourceStatus.StatusesOrder = destOrder;
        }
        else if (sourceOrder > destOrder)
        {
            statuses
                .Where(s => s.StatusesOrder >= destOrder && s.StatusesOrder < sourceOrder)
                .ToList()
                .ForEach(s => s.StatusesOrder++);

            sourceStatus.StatusesOrder = destOrder;
        }

        await _context.SaveChangesAsync();

        var result = await _context.Statuses
            .OrderBy(s => s.StatusesOrder)
            .Select(s => new StatusDto
            {
                StatusId = s.StatusId,
                StatusName = s.StatusName,
                HexCode = s.HexCode
            })
            .ToListAsync();

        return new ServiceResult<List<StatusDto>>(ServiceStatus.Ok, string.Empty, result);
    }


    public async Task<ServiceResult<StatusDto>> PostStatus(StatusDto statusDto)
    {
        var order = _context.Statuses.Count() + 1;
        var status = new Status
        {
            StatusName = statusDto.StatusName,
            HexCode = statusDto.HexCode,
            StatusesOrder = (short)order
        };
        _context.Add(status);
        await _context.SaveChangesAsync();
        var result = new StatusDto
        {
            StatusId = status.StatusId,
            StatusName = status.StatusName,
        };
        return new ServiceResult<StatusDto>(ServiceStatus.Ok, string.Empty, result);
    }

    public async Task<ServiceResult<StatusDto>> UpdateStatus(short id, StatusDto newStatus)
    {
        var oldStatus = await _context.Statuses.FindAsync(id);
        if (oldStatus == null)
        {
            return ServiceResult<StatusDto>.NotFound("Nie znaleziono statusu");
        }

        oldStatus.StatusName = newStatus.StatusName;
        oldStatus.HexCode = newStatus.HexCode;
        await _context.SaveChangesAsync();
        var result = new StatusDto
        {
            StatusId = oldStatus.StatusId,
            StatusName = oldStatus.StatusName,
        };
        return new ServiceResult<StatusDto>(ServiceStatus.Ok, string.Empty, result);
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