using ams_desk_cs_backend.BikeFilters.Dtos;
using ams_desk_cs_backend.BikeFilters.Interfaces;
using ams_desk_cs_backend.Data;
using ams_desk_cs_backend.Data.Models;
using ams_desk_cs_backend.Shared.Results;
using Microsoft.EntityFrameworkCore;
using Exception = System.Exception;

namespace ams_desk_cs_backend.BikeFilters.Services;

public class ColorsService : IColorsService
{
    private readonly BikesDbContext _context;
    public ColorsService(BikesDbContext dbContext)
    {
        _context = dbContext;
    }



    public async Task<ServiceResult<ColorDto>> GetColor(short id)
    {
        var existingColor = await _context.Colors.FindAsync(id);
        if (existingColor == null)
        {
            return ServiceResult<ColorDto>.NotFound("Nie znaleziono koloru");
        }
        return new ServiceResult<ColorDto>(ServiceStatus.Ok, string.Empty,
            new ColorDto
            {
                Id = existingColor.Id,
                Name = existingColor.Name,
                Color = existingColor.Color,
            });
    }

    public async Task<ServiceResult<IEnumerable<ColorDto>>> GetColors()
    {
        var colors = await _context.Colors.OrderBy(c => c.Order).Select(color => new ColorDto
        {
            Id = color.Id,
            Name = color.Name,
            Color = color.Color,
        }).ToListAsync();
        return new ServiceResult<IEnumerable<ColorDto>>(ServiceStatus.Ok, string.Empty, colors);
    }
    public async Task<ServiceResult<ColorDto>> PostColor(ColorDto colorDto)
    {
        var order = _context.Colors.Count() + 1;
        var color = new ModelColor
        {
            Name = colorDto.Name,
            Color = colorDto.Color,
            Order = (short)order
        };
        _context.Add(color);
        await _context.SaveChangesAsync();
        var result = new ColorDto
        {
            Id = color.Id,
            Name = color.Name,
            Color = color.Color,
        };
        return new ServiceResult<ColorDto>(ServiceStatus.Ok, string.Empty, result);
    }
    public async Task<ServiceResult<ColorDto>> UpdateColor(short id, ColorDto newColor)
    {
        var oldColor = await _context.Colors.FindAsync(id);
        if (oldColor == null)
        {
            return ServiceResult<ColorDto>.NotFound("Nie znaleziono koloru");
        }

        oldColor.Name = newColor.Name;
        oldColor.Color = newColor.Color;
        await _context.SaveChangesAsync();
        var result = new ColorDto
        {
            Id = oldColor.Id,
            Name = oldColor.Name,
            Color = oldColor.Color,
        };
        return new ServiceResult<ColorDto>(ServiceStatus.Ok, string.Empty, result);
    }

    public async Task<ServiceResult<List<ColorDto>>> ChangeOrder(short source, short dest)
    {
        if (!_context.Colors.Any(c => c.Id == source || c.Id == dest))
        {
            return ServiceResult<List<ColorDto>>.NotFound("Nie znaleziono zamienianych elementów");
        }

        var colors = await _context.Colors.OrderBy(c => c.Order).ToListAsync();
        var sourceColor = colors.First(c => c.Id == source);
        var destColor = colors.First(c => c.Id == dest);

        var sourceOrder = sourceColor.Order;
        var destOrder = destColor.Order;

        if (sourceOrder < destOrder)
        {
            colors
                .Where(c => c.Order > sourceOrder && c.Order <= destOrder)
                .ToList()
                .ForEach(c => c.Order--);

            sourceColor.Order = destOrder;
        }
        else if (sourceOrder > destOrder)
        {
            colors
                .Where(c => c.Order >= destOrder && c.Order < sourceOrder)
                .ToList()
                .ForEach(c => c.Order++);

            sourceColor.Order = destOrder;
        }

        await _context.SaveChangesAsync();

        var result = await _context.Colors
            .OrderBy(c => c.Order)
            .Select(c => new ColorDto
            {
                Id = c.Id,
                Name = c.Name,
                Color = c.Color
            })
            .ToListAsync();

        return new ServiceResult<List<ColorDto>>(ServiceStatus.Ok, string.Empty, result);
    }


    public async Task<ServiceResult> DeleteColor(short id)
    {
        try
        {
            var existingColor = await _context.Colors.FindAsync(id);
            if (existingColor == null)
            {
                return ServiceResult<ColorDto>.NotFound("Nie znaleziono koloru");
            }
            _context.Colors.Remove(existingColor);
            await _context.SaveChangesAsync();
            return new ServiceResult(ServiceStatus.Ok, string.Empty);
        }
        catch (Exception ex)
        {
            return new ServiceResult(ServiceStatus.BadRequest, "Kolor jest przypisany do modelu");
        }
    }
}