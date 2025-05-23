using ams_desk_cs_backend.BikeFilters.Dtos;
using ams_desk_cs_backend.BikeFilters.Interfaces;
using ams_desk_cs_backend.Data;
using ams_desk_cs_backend.Data.Models;
using ams_desk_cs_backend.Shared.Results;
using Microsoft.EntityFrameworkCore;

namespace ams_desk_cs_backend.BikeFilters.Services;


public class WheelSizesService : IWheelSizesService
{
    private readonly BikesDbContext _context;
    public WheelSizesService(BikesDbContext context)
    {
        _context = context;
    }
    public async Task<ServiceResult<IEnumerable<WheelSizeDto>>> GetWheelSizes()
    {
        var wheelSizes = await _context.WheelSizes.Select(wheelSize => wheelSize.WheelSizeId).OrderBy(wheelSize => wheelSize).ToListAsync();
        var wheelSizesDto = wheelSizes.Select(value => new WheelSizeDto { Key = value, Value = value });
        return new ServiceResult<IEnumerable<WheelSizeDto>>(ServiceStatus.Ok, string.Empty, wheelSizesDto);
    }

    public async Task<ServiceResult<WheelSizeDto>> PostWheelSize(decimal wheelSize)
    {
        var existingWheelSize = await _context.WheelSizes.FindAsync(wheelSize);
        if (existingWheelSize != null && wheelSize != 0)
        {
            return ServiceResult<WheelSizeDto>.BadRequest("Rozmiar koła już istnieje");
        }
        _context.WheelSizes.Add(new WheelSize { WheelSizeId = wheelSize });
        await _context.SaveChangesAsync();
        return new ServiceResult<WheelSizeDto>(
            ServiceStatus.Ok, 
            string.Empty, 
            new WheelSizeDto { Key = wheelSize, Value = wheelSize }
            );
    }
    public async Task<ServiceResult> DeleteWheelSize(decimal wheelSize)
    {
        try
        {
            var existingWheelSize = await _context.WheelSizes.FindAsync(wheelSize);
            if (existingWheelSize == null)
            {
                return new ServiceResult(ServiceStatus.NotFound, "Rozmiar koła nie istnieje");
            }
            _context.WheelSizes.Remove(existingWheelSize);
            await _context.SaveChangesAsync();
            return new ServiceResult(ServiceStatus.Ok, string.Empty);
        }
        catch (Exception ex)
        {
            return new ServiceResult(ServiceStatus.BadRequest, "Rozmiar koła przypisany do rowerów");
        }
    }
}