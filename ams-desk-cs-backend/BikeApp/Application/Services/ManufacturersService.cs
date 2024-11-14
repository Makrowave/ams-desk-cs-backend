using ams_desk_cs_backend.BikeApp.Application.Interfaces;
using ams_desk_cs_backend.BikeApp.Application.Interfaces.Validators;
using ams_desk_cs_backend.BikeApp.Dtos.AppModelDto;
using ams_desk_cs_backend.BikeApp.Infrastructure.Data;
using ams_desk_cs_backend.BikeApp.Infrastructure.Data.Models;
using ams_desk_cs_backend.Shared.Results;
using Microsoft.EntityFrameworkCore;

namespace ams_desk_cs_backend.BikeApp.Application.Services
{
    public class ManufacturersService : IManufacturersService
    {
        private readonly BikesDbContext _context;
        private readonly ICommonValidator _commonValidator;
        public ManufacturersService(BikesDbContext context, ICommonValidator commonValidator)
        {
            _context = context;
            _commonValidator = commonValidator;
        }
        public async Task<ServiceResult<IEnumerable<ManufacturerDto>>> GetManufacturers()
        {
            var manufacturers = await _context.Manufacturers.Select(manufacter => new ManufacturerDto
            {
                ManufacturerId = manufacter.ManufacturerId,
                ManufacturerName = manufacter.ManufacturerName,

            }).ToListAsync();
            return new ServiceResult<IEnumerable<ManufacturerDto>>(ServiceStatus.Ok, string.Empty, manufacturers);
        }

        public async Task<ServiceResult> PostManufacturer(ManufacturerDto manufacturer)
        {
            if (manufacturer.ManufacturerName == null ||
                !_commonValidator.Validate16CharNameAnyCase(manufacturer.ManufacturerName))
            {
                return new ServiceResult(ServiceStatus.BadRequest, "Zła nazwa producenta");
            }
            _context.Add(new Manufacturer
            {
                ManufacturerName = manufacturer.ManufacturerName,
            });
            await _context.SaveChangesAsync();
            return new ServiceResult(ServiceStatus.Ok, string.Empty);
        }

        public async Task<ServiceResult> UpdateManufacturer(short id, ManufacturerDto manufacturer)
        {
            var existingManufacturer = await _context.Manufacturers.FindAsync(id);
            if (existingManufacturer == null)
            {
                return new ServiceResult(ServiceStatus.NotFound, "Nie znaleziono producenta");
            }
            if (manufacturer.ManufacturerName != null &&
                _commonValidator.Validate16CharNameAnyCase(manufacturer.ManufacturerName))
            {
                existingManufacturer.ManufacturerName = manufacturer.ManufacturerName;
            }
            await _context.SaveChangesAsync();
            return new ServiceResult(ServiceStatus.Ok, string.Empty);
        }
        public async Task<ServiceResult> DeleteManufacturer(short id)
        {
            try
            {
                var existingManufacturer = await _context.Manufacturers.FindAsync(id);
                if (existingManufacturer == null)
                {
                    return new ServiceResult(ServiceStatus.NotFound, "Nie znaleziono producenta");
                }
                _context.Manufacturers.Remove(existingManufacturer);
                await _context.SaveChangesAsync();
                return new ServiceResult(ServiceStatus.Ok, string.Empty);
            }
            catch (Exception ex)
            {
                return new ServiceResult(ServiceStatus.BadRequest, "Producent jest przypisany do modelu");
            }
        }
    }
}
