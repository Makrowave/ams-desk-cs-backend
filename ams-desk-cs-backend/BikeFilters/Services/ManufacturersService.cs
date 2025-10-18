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
        var manufacturers = await _context.Manufacturers.OrderBy(manufacturer => manufacturer.ManufacturersOrder)
            .Select(manufacter => new ManufacturerDto
            {
                Id = manufacter.ManufacturerId,
                Name = manufacter.ManufacturerName,
            }).ToListAsync();
        return new ServiceResult<IEnumerable<ManufacturerDto>>(ServiceStatus.Ok, string.Empty, manufacturers);
    }

    public async Task<ServiceResult<ManufacturerDto>> PostManufacturer(ManufacturerDto manufacturerDto)
    {
        var order = _context.Manufacturers.Count() + 1;
        var manufacturer = new Manufacturer
        {
            ManufacturerName = manufacturerDto.Name,
            ManufacturersOrder = (short)order
        };
        _context.Add(manufacturer);
        await _context.SaveChangesAsync();
        var result = new ManufacturerDto
        {
            Id = manufacturer.ManufacturerId,
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

        oldManufacturer.ManufacturerName = newManufacturer.Name;
        await _context.SaveChangesAsync();
        var result = new ManufacturerDto
        {
            Id = oldManufacturer.ManufacturerId,
            Name = oldManufacturer.ManufacturerName,
        };
        return new ServiceResult<ManufacturerDto>(ServiceStatus.Ok, string.Empty, result);
    }

    public async Task<ServiceResult<List<ManufacturerDto>>> ChangeOrder(short source, short dest)
    {
        if (!_context.Manufacturers.Any(m => m.ManufacturerId == source || m.ManufacturerId == dest))
        {
            return ServiceResult<List<ManufacturerDto>>.NotFound("Nie znaleziono zamienianych elementów");
        }

        var manufacturers = await _context.Manufacturers.OrderBy(m => m.ManufacturersOrder).ToListAsync();
        var sourceManufacturer = manufacturers.First(m => m.ManufacturerId == source);
        var destManufacturer = manufacturers.First(m => m.ManufacturerId == dest);

        var sourceOrder = sourceManufacturer.ManufacturersOrder;
        var destOrder = destManufacturer.ManufacturersOrder;

        if (sourceOrder < destOrder)
        {
            manufacturers
                .Where(m => m.ManufacturersOrder > sourceOrder && m.ManufacturersOrder <= destOrder)
                .ToList()
                .ForEach(m => m.ManufacturersOrder--);

            sourceManufacturer.ManufacturersOrder = destOrder;
        }
        else if (sourceOrder > destOrder)
        {
            manufacturers
                .Where(m => m.ManufacturersOrder >= destOrder && m.ManufacturersOrder < sourceOrder)
                .ToList()
                .ForEach(m => m.ManufacturersOrder++);

            sourceManufacturer.ManufacturersOrder = destOrder;
        }

        await _context.SaveChangesAsync();

        var result = await _context.Manufacturers
            .OrderBy(m => m.ManufacturersOrder)
            .Select(m => new ManufacturerDto
            {
                Id = m.ManufacturerId,
                Name = m.ManufacturerName
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