using ams_desk_cs_backend.BikeApp.Application.Interfaces;
using ams_desk_cs_backend.BikeApp.Dtos.AppModelDto;
using ams_desk_cs_backend.BikeApp.Infrastructure.Data;
using ams_desk_cs_backend.BikeApp.Infrastructure.Data.Models;
using ams_desk_cs_backend.Shared.Results;
using Microsoft.EntityFrameworkCore;

namespace ams_desk_cs_backend.BikeApp.Application.Services
{
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
                return new ServiceResult<ColorDto>(ServiceStatus.NotFound, "Nie znaleziono koloru", null);
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
        public async Task<ServiceResult> PostColor(ColorDto color)
        {
            var order = _context.Colors.Count() + 1;
            _context.Add(new Color
            {
                ColorName = color.ColorName,
                HexCode = color.HexCode,
                ColorsOrder = (short) order
            });
            await _context.SaveChangesAsync();
            return new ServiceResult(ServiceStatus.Ok, string.Empty);
        }
        public async Task<ServiceResult> UpdateColor(short id, ColorDto newColor)
        {
            var oldColor = await _context.Colors.FindAsync(id);
            if (oldColor == null)
            {
                return new ServiceResult(ServiceStatus.NotFound, "Nie znaleziono koloru");
            }

            oldColor.ColorName = newColor.ColorName;
            oldColor.HexCode = newColor.HexCode;
            await _context.SaveChangesAsync();
            return new ServiceResult(ServiceStatus.Ok, string.Empty);
        }

        public async Task<ServiceResult> ChangeOrder(short firstId, short lastId)
        {
            if(!_context.Colors.Any(color => (color.ColorId == firstId || color.ColorId == lastId)))
            {
                return new ServiceResult(ServiceStatus.NotFound, "Nie znaleziono zamienianych elementów");
            }
            var colors = await _context.Colors.OrderBy(color => color.ColorsOrder).ToListAsync();
            var firstOrder = colors.FirstOrDefault(color => color.ColorId == firstId)!.ColorsOrder;
            var lastOrder = colors.FirstOrDefault(color => color.ColorId == lastId)!.ColorsOrder;

            var filteredColors = colors.Where(color => color.ColorsOrder >= firstOrder && color.ColorsOrder <= lastOrder).ToList();
            filteredColors.ForEach(color => color.ColorsOrder++);
            filteredColors.Last().ColorsOrder = firstOrder;
            await _context.SaveChangesAsync();
            return new ServiceResult(ServiceStatus.Ok, string.Empty);
        }

        public async Task<ServiceResult> DeleteColor(short id)
        {
            try
            {
                var existingColor = await _context.Colors.FindAsync(id);
                if (existingColor == null)
                {
                    return new ServiceResult(ServiceStatus.NotFound, "Nie znaleziono koloru");
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
}
