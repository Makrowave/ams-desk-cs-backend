using ams_desk_cs_backend.BikeApp.Application.Interfaces;
using ams_desk_cs_backend.BikeApp.Application.Interfaces.Validators;
using ams_desk_cs_backend.BikeApp.Dtos.AppModelDto;
using ams_desk_cs_backend.BikeApp.Infrastructure.Data;
using ams_desk_cs_backend.BikeApp.Infrastructure.Data.Models;
using ams_desk_cs_backend.Shared.Results;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace ams_desk_cs_backend.BikeApp.Application.Services
{
    public class ColorsService : IColorsService
    {
        private readonly BikesDbContext _context;
        private readonly ICommonValidator _commonValidator;
        public ColorsService(BikesDbContext dbContext, ICommonValidator commonValidator)
        {
            _context = dbContext;
            _commonValidator = commonValidator;
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
        public async Task<ServiceResult> PostColor(ColorDto color)
        {
            if (color.ColorName == null || _commonValidator.Validate16CharName(color.ColorName))
            {
                return new ServiceResult(ServiceStatus.BadRequest, "Zła nazwa koloru");
            }
            if (color.HexCode == null || _commonValidator.ValidateColor(color.HexCode))
            {
                return new ServiceResult(ServiceStatus.BadRequest, "Zła nazwa koloru");
            }
            _context.Add(new Color
            {
                ColorName = color.ColorName,
                HexCode = color.HexCode,
            });
            await _context.SaveChangesAsync();
            return new ServiceResult(ServiceStatus.Ok, string.Empty);
        }
        public async Task<ServiceResult> UpdateColor(short id, ColorDto color)
        {
            var existingColor = await _context.Colors.FindAsync(id);
            if(existingColor == null)
            {
                return new ServiceResult(ServiceStatus.NotFound, "Nie znaleziono roweru");
            }
            if(color.ColorName != null && _commonValidator.Validate16CharName(color.ColorName))
            {
                existingColor.ColorName = color.ColorName;
            }
            if (color.HexCode != null && _commonValidator.ValidateColor(color.HexCode))
            {
                existingColor.HexCode = color.HexCode;
            }
            await _context.SaveChangesAsync();
            return new ServiceResult(ServiceStatus.Ok, "Saved changes");
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
