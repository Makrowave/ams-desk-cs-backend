using ams_desk_cs_backend.BikeFilters.Dtos;
using ams_desk_cs_backend.BikeFilters.Interfaces;
using ams_desk_cs_backend.Data;
using ams_desk_cs_backend.Data.Models;
using ams_desk_cs_backend.Shared.Results;
using Microsoft.EntityFrameworkCore;

namespace ams_desk_cs_backend.BikeFilters.Services;

public class ManufacturersService : IManufacturersService
{
    private readonly BikesDbContext _context;

    public ManufacturersService(BikesDbContext context)
    {
        _context = context;
    }

    public async Task<ServiceResult<IEnumerable<ManufacturerDto>>> GetManufacturers()
    {
        var manufacturers = await _context.Manufacturers.OrderBy(manufacturer => manufacturer.Order)
            .Select(manufacter => new ManufacturerDto
            {
                Id = manufacter.Id,
                Name = manufacter.Name,
            }).ToListAsync();
        return new ServiceResult<IEnumerable<ManufacturerDto>>(ServiceStatus.Ok, string.Empty, manufacturers);
    }

    public async Task<ServiceResult<ManufacturerDto>> PostManufacturer(ManufacturerDto manufacturerDto)
    {
        var order = _context.Manufacturers.Count() + 1;
        var manufacturer = new Manufacturer
        {
            Name = manufacturerDto.Name,
            Order = (short)order
        };
        _context.Add(manufacturer);
        await _context.SaveChangesAsync();
        var result = new ManufacturerDto
        {
            Id = manufacturer.Id,
            Name = manufacturerDto.Name,
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

        oldManufacturer.Name = newManufacturer.Name;
        await _context.SaveChangesAsync();
        var result = new ManufacturerDto
        {
            Id = oldManufacturer.Id,
            Name = oldManufacturer.Name,
        };
        return new ServiceResult<ManufacturerDto>(ServiceStatus.Ok, string.Empty, result);
    }

    public async Task<ServiceResult<List<ManufacturerDto>>> ChangeOrder(short source, short dest)
    {
        if (!_context.Manufacturers.Any(m => m.Id == source || m.Id == dest))
        {
            return ServiceResult<List<ManufacturerDto>>.NotFound("Nie znaleziono zamienianych elementów");
        }

        var manufacturers = await _context.Manufacturers.OrderBy(m => m.Order).ToListAsync();
        var sourceManufacturer = manufacturers.First(m => m.Id == source);
        var destManufacturer = manufacturers.First(m => m.Id == dest);

        var sourceOrder = sourceManufacturer.Order;
        var destOrder = destManufacturer.Order;

        if (sourceOrder < destOrder)
        {
            manufacturers
                .Where(m => m.Order > sourceOrder && m.Order <= destOrder)
                .ToList()
                .ForEach(m => m.Order--);

            sourceManufacturer.Order = destOrder;
        }
        else if (sourceOrder > destOrder)
        {
            manufacturers
                .Where(m => m.Order >= destOrder && m.Order < sourceOrder)
                .ToList()
                .ForEach(m => m.Order++);

            sourceManufacturer.Order = destOrder;
        }

        await _context.SaveChangesAsync();

        var result = await _context.Manufacturers
            .OrderBy(m => m.Order)
            .Select(m => new ManufacturerDto
            {
                Id = m.Id,
                Name = m.Name
            })
            .ToListAsync();

        return new ServiceResult<List<ManufacturerDto>>(ServiceStatus.Ok, string.Empty, result);
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