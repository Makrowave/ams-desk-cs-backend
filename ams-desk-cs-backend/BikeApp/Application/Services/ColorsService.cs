using ams_desk_cs_backend.BikeApp.Application.Interfaces;
using ams_desk_cs_backend.BikeApp.Dtos.AppModelDto;
using ams_desk_cs_backend.BikeApp.Infrastructure.Data;
using ams_desk_cs_backend.Shared.Results;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

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
            var colors = await _context.Colors.OrderBy(c => c.ColorId).Select(color => new ColorDto
            {
                ColorId = color.ColorId, 
                ColorName = color.ColorName, 
                HexCode = color.HexCode,
            }).ToListAsync();
            return new ServiceResult<IEnumerable<ColorDto>> (ServiceStatus.Ok, string.Empty, colors);
        }
    }
}
