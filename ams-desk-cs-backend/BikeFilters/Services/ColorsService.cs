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
                ColorId = existingColor.ColorId,
                ColorName = existingColor.ColorName,
                HexCode = existingColor.HexCode,
            });
    }

    public async Task<ServiceResult<IEnumerable<ColorDto>>> GetColors()
    {
        var colors = await _context.Colors.OrderBy(c => c.ColorsOrder).Select(color => new ColorDto
        {
            ColorId = color.ColorId,
            ColorName = color.ColorName,
            HexCode = color.HexCode,
        }).ToListAsync();
        return new ServiceResult<IEnumerable<ColorDto>>(ServiceStatus.Ok, string.Empty, colors);
    }
    public async Task<ServiceResult<ColorDto>> PostColor(ColorDto colorDto)
    {
        var order = _context.Colors.Count() + 1;
        var color = new Color
        {
            ColorName = colorDto.ColorName,
            HexCode = colorDto.HexCode,
            ColorsOrder = (short)order
        };
        _context.Add(color);
        await _context.SaveChangesAsync();
        var result = new ColorDto
        {
            ColorId = color.ColorId,
            ColorName = color.ColorName,
            HexCode = color.HexCode,
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

        oldColor.ColorName = newColor.ColorName;
        oldColor.HexCode = newColor.HexCode;
        await _context.SaveChangesAsync();
        var result = new ColorDto
        {
            ColorId = oldColor.ColorId,
            ColorName = oldColor.ColorName,
            HexCode = oldColor.HexCode,
        };
        return new ServiceResult<ColorDto>(ServiceStatus.Ok, string.Empty, result);
    }

    public async Task<ServiceResult<List<ColorDto>>> ChangeOrder(short source, short dest)
    {
        if (!_context.Colors.Any(c => c.ColorId == source || c.ColorId == dest))
        {
            return ServiceResult<List<ColorDto>>.NotFound("Nie znaleziono zamienianych elementów");
        }

        var colors = await _context.Colors.OrderBy(c => c.ColorsOrder).ToListAsync();
        var sourceColor = colors.First(c => c.ColorId == source);
        var destColor = colors.First(c => c.ColorId == dest);

        var sourceOrder = sourceColor.ColorsOrder;
        var destOrder = destColor.ColorsOrder;

        if (sourceOrder < destOrder)
        {
            colors
                .Where(c => c.ColorsOrder > sourceOrder && c.ColorsOrder <= destOrder)
                .ToList()
                .ForEach(c => c.ColorsOrder--);

            sourceColor.ColorsOrder = destOrder;
        }
        else if (sourceOrder > destOrder)
        {
            colors
                .Where(c => c.ColorsOrder >= destOrder && c.ColorsOrder < sourceOrder)
                .ToList()
                .ForEach(c => c.ColorsOrder++);

            sourceColor.ColorsOrder = destOrder;
        }

        await _context.SaveChangesAsync();

        var result = await _context.Colors
            .OrderBy(c => c.ColorsOrder)
            .Select(c => new ColorDto
            {
                ColorId = c.ColorId,
                ColorName = c.ColorName,
                HexCode = c.HexCode
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