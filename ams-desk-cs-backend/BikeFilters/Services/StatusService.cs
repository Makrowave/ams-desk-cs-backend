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
            Id = status.Id,
            Name = status.Name,
            Color = status.Color,
        });
    }

    public async Task<ServiceResult<IEnumerable<StatusDto>>> GetStatuses()
    {
        var statuses = await _context.Statuses.OrderBy(status => status.Order)
            .Select(status => new StatusDto
            {
                Id = status.Id,
                Name = status.Name,
                Color = status.Color,
            }).ToListAsync();
        return new ServiceResult<IEnumerable<StatusDto>>(ServiceStatus.Ok, string.Empty, statuses);
    }

    public async Task<ServiceResult<IEnumerable<StatusDto>>> GetStatusesExcluded(int[] excludedStatuses)
    {
        var statuses = await _context.Statuses
            .Where(status => excludedStatuses.All(s => s != status.Id))
            .OrderBy(status => status.Order)
            .Select(status => new StatusDto
            {
                Id = status.Id,
                Name = status.Name,
                Color = status.Color,
            }).OrderBy(status => status.Id).ToListAsync();
        return new ServiceResult<IEnumerable<StatusDto>>(ServiceStatus.Ok, string.Empty, statuses);
    }
    public async Task<ServiceResult<List<StatusDto>>> ChangeOrder(short source, short dest)
    {
        if (!_context.Statuses.Any(s => s.Id == source || s.Id == dest))
        {
            return ServiceResult<List<StatusDto>>.NotFound("Nie znaleziono zamienianych elementów");
        }

        var statuses = await _context.Statuses.OrderBy(s => s.Order).ToListAsync();
        var sourceStatus = statuses.First(s => s.Id == source);
        var destStatus = statuses.First(s => s.Id == dest);

        var sourceOrder = sourceStatus.Order;
        var destOrder = destStatus.Order;

        if (sourceOrder < destOrder)
        {
            statuses
                .Where(s => s.Order > sourceOrder && s.Order <= destOrder)
                .ToList()
                .ForEach(s => s.Order--);

            sourceStatus.Order = destOrder;
        }
        else if (sourceOrder > destOrder)
        {
            statuses
                .Where(s => s.Order >= destOrder && s.Order < sourceOrder)
                .ToList()
                .ForEach(s => s.Order++);

            sourceStatus.Order = destOrder;
        }

        await _context.SaveChangesAsync();

        var result = await _context.Statuses
            .OrderBy(s => s.Order)
            .Select(s => new StatusDto
            {
                Id = s.Id,
                Name = s.Name,
                Color = s.Color
            })
            .ToListAsync();

        return new ServiceResult<List<StatusDto>>(ServiceStatus.Ok, string.Empty, result);
    }


    public async Task<ServiceResult<StatusDto>> PostStatus(StatusDto statusDto)
    {
        var order = _context.Statuses.Count() + 1;
        var status = new Status
        {
            Name = statusDto.Name,
            Color = statusDto.Color,
            Order = (short)order
        };
        _context.Add(status);
        await _context.SaveChangesAsync();
        var result = new StatusDto
        {
            Id = status.Id,
            Name = status.Name,
            Color = status.Color,
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

        oldStatus.Name = newStatus.Name;
        oldStatus.Color = newStatus.Color;
        await _context.SaveChangesAsync();
        var result = new StatusDto
        {
            Id = oldStatus.Id,
            Name = oldStatus.Name,
            Color = oldStatus.Color,
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