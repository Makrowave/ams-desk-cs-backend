using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ams_desk_cs_backend.BikeApp.Application.Interfaces;
using ams_desk_cs_backend.BikeApp.Infrastructure.Data;
using ams_desk_cs_backend.Shared.Results;
using Microsoft.EntityFrameworkCore;

namespace ams_desk_cs_backend.BikeApp.Application.Services;

[Authorize(Policy = "AccessToken")]
[Route("api/[controller]")]
[ApiController]
public class WheelSizesService : IWheelSizesService
{
    private readonly BikesDbContext _context;
    public WheelSizesService(BikesDbContext context)
    {
        _context = context;
    }
    public async Task<ServiceResult<IEnumerable<short>>> GetWheelSizes()
    {
        var wheelSizes = await _context.WheelSizes.Select(wheelSize => wheelSize.WheelSizeId).ToListAsync();
        return new ServiceResult<IEnumerable<short>>(ServiceStatus.Ok, string.Empty, wheelSizes);
    }

    public async Task<ServiceResult> PostWheelSize(short wheelSize)
    {
        var existingWheelSize = await _context.WheelSizes.FindAsync(wheelSize);
        if (existingWheelSize != null)
        {
            return new ServiceResult(ServiceStatus.BadRequest, "Rozmiar koła już istnieje");
        }
        return new ServiceResult(ServiceStatus.Ok, string.Empty);
    }
    public async Task<ServiceResult> DeleteWheelSize(short wheelSize)
    {
        try
        {
            var existingWheelSize = await _context.WheelSizes.FindAsync(wheelSize);
            if (existingWheelSize == null)
            {
                return new ServiceResult(ServiceStatus.NotFound, "Rozmiar koła nie istnieje");
            }
            _context.WheelSizes.Remove(existingWheelSize);
            return new ServiceResult(ServiceStatus.Ok, string.Empty);
        }
        catch (Exception ex)
        {
            return new ServiceResult(ServiceStatus.BadRequest, "Rozmiar koła przypisany do rowerów");
        }
    }
}