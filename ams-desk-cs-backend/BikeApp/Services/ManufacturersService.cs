using ams_desk_cs_backend.BikeApp.Data;
using ams_desk_cs_backend.BikeApp.Dtos.AppModelDto;
using ams_desk_cs_backend.BikeApp.Data.Models;
using ams_desk_cs_backend.BikeApp.Interfaces;
using ams_desk_cs_backend.Shared.Results;
using Microsoft.EntityFrameworkCore;

namespace ams_desk_cs_backend.BikeApp.Services
{
    public class ManufacturersService : IManufacturersService
    {
        private readonly BikesDbContext _context;

        public ManufacturersService(BikesDbContext context)
        {
            _context = context;
        }

        public async Task<ServiceResult<IEnumerable<ManufacturerDto>>> GetManufacturers()
        {
            var manufacturers = await _context.Manufacturers.OrderBy(manufacturer => manufacturer.ManufacturersOrder)
                .Select(manufacter => new ManufacturerDto
                {
                    ManufacturerId = manufacter.ManufacturerId,
                    ManufacturerName = manufacter.ManufacturerName,
                }).ToListAsync();
            return new ServiceResult<IEnumerable<ManufacturerDto>>(ServiceStatus.Ok, string.Empty, manufacturers);
        }

        public async Task<ServiceResult<ManufacturerDto>> PostManufacturer(ManufacturerDto manufacturerDto)
        {
            var order = _context.Manufacturers.Count() + 1;
            var manufacturer = new Manufacturer
            {
                ManufacturerName = manufacturerDto.ManufacturerName,
                ManufacturersOrder = (short)order
            };
            _context.Add(manufacturer);
            await _context.SaveChangesAsync();
            var result = new ManufacturerDto
            {
                ManufacturerId = manufacturer.ManufacturerId,
                ManufacturerName = manufacturerDto.ManufacturerName,
            };
            return new ServiceResult<ManufacturerDto>(ServiceStatus.Ok, string.Empty, result);
        }

        public async Task<ServiceResult<ManufacturerDto>> UpdateManufacturer(short id, ManufacturerDto newManufacturer)
        {
            var oldManufacturer = await _context.Manufacturers.FindAsync(id);
            if (oldManufacturer == null)
            {
                return ServiceResult<ManufacturerDto>.NotFound("Nie znaleziono producenta");
            }

            oldManufacturer.ManufacturerName = newManufacturer.ManufacturerName;
            await _context.SaveChangesAsync();
            var result = new ManufacturerDto
            {
                ManufacturerId = oldManufacturer.ManufacturerId,
                ManufacturerName = oldManufacturer.ManufacturerName,
            };
            return new ServiceResult<ManufacturerDto>(ServiceStatus.Ok, string.Empty, result);
        }

        public async Task<ServiceResult> ChangeOrder(short firstId, short lastId)
        {
            if (!_context.Manufacturers.Any(manufacturer =>
                    manufacturer.ManufacturerId == firstId 
                    || manufacturer.ManufacturerId == lastId))
            {
                return new ServiceResult(ServiceStatus.NotFound, "Nie znaleziono zamienianych elementów");
            }

            var manufacturers = await _context.Manufacturers
                .OrderBy(manufacturer => manufacturer.ManufacturersOrder)
                .ToListAsync();
            var firstOrder = manufacturers.
                FirstOrDefault(manufacturer => manufacturer.ManufacturerId == firstId)!
                .ManufacturersOrder;
            var lastOrder = manufacturers
                .FirstOrDefault(manufacturer => manufacturer.ManufacturerId == lastId)!
                .ManufacturersOrder;

            var filteredManufacturers =
                manufacturers.Where(manufacturer =>
                        manufacturer.ManufacturersOrder >= firstOrder && manufacturer.ManufacturersOrder <= lastOrder)
                    .ToList();
            filteredManufacturers.ForEach(manufacturer => manufacturer.ManufacturersOrder++);
            filteredManufacturers.Last().ManufacturersOrder = firstOrder;
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