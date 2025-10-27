using ams_desk_cs_backend.Data;
using ams_desk_cs_backend.Data.Models;
using ams_desk_cs_backend.Places.Dtos;
using ams_desk_cs_backend.Places.Interfaces;
using ams_desk_cs_backend.Shared.Results;
using Microsoft.EntityFrameworkCore;

namespace ams_desk_cs_backend.Places.Services;

public class PlaceService : IPlacesService
{
    private readonly BikesDbContext _context;
    public PlaceService(BikesDbContext context)
    {
        _context = context;
    }
    public async Task<ServiceResult<IEnumerable<PlaceDto>>> GetPlaces()
    {
        var places = await _context.Places.OrderBy(place => place.Order)
            .Select(place => new PlaceDto(place)).ToListAsync();
        return new ServiceResult<IEnumerable<PlaceDto>>(ServiceStatus.Ok, string.Empty, places);
    }

    public async Task<ServiceResult<IEnumerable<PlaceDto>>> GetPlacesNotStorage()
    {
        var places = await _context.Places.OrderBy(place => place.Order)
            .Where(place => !place.IsStorage)
            .Select(place => new PlaceDto(place)).ToListAsync();
        return new ServiceResult<IEnumerable<PlaceDto>>(ServiceStatus.Ok, string.Empty, places);
    }

    public async Task<ServiceResult<List<PlaceDto>>> ChangeOrder(short source, short dest)
    {
        if (!_context.Places.Any(p => p.Id == source || p.Id == dest))
        {
            return ServiceResult<List<PlaceDto>>.NotFound("Nie znaleziono zamienianych elementów");
        }

        var places = await _context.Places.OrderBy(p => p.Order).ToListAsync();

        var firstPlace = places.First(p => p.Id == source);
        var lastPlace = places.First(p => p.Id == dest);
        var firstOrder = firstPlace.Order;
        var lastOrder = lastPlace.Order;

        if (firstOrder < lastOrder)
        {
            var toShift = places
                .Where(p => p.Order > firstOrder && p.Order <= lastOrder)
                .ToList();

            toShift.ForEach(p => p.Order--);
            firstPlace.Order = lastOrder;
        }
        else if (firstOrder > lastOrder)
        {
            var toShift = places
                .Where(p => p.Order >= lastOrder && p.Order < firstOrder)
                .ToList();

            toShift.ForEach(p => p.Order++);
            firstPlace.Order = lastOrder;
        }

        await _context.SaveChangesAsync();

        var result = await _context.Places
            .OrderBy(p => p.Order)
            .Select(p => new PlaceDto(p))
            .ToListAsync();

        return new ServiceResult<List<PlaceDto>>(ServiceStatus.Ok, string.Empty, result);
    }


    public async Task<ServiceResult<PlaceDto>> PostPlace(PlaceDto placeDto)
    {
        var places = await _context.Places.OrderBy(place => place.Order).ToListAsync();
        var oldPlace = places.FirstOrDefault(place => place.Id == placeDto.Id);
        if (oldPlace != null)
        {
            return ServiceResult<PlaceDto>.BadRequest("Miejsce już istnieje");
        }

        var place = new Place
        {
            Name = placeDto.Name,
            IsStorage = placeDto.IsStorage,
            Order = (short)((places.LastOrDefault()?.Order ?? 0) + 1)
        };
        _context.Places.Add(place);
        await _context.SaveChangesAsync();
        return new ServiceResult<PlaceDto>(ServiceStatus.Ok, string.Empty, new PlaceDto(place));
    }

    public async Task<ServiceResult<PlaceDto>> PutPlace(short id, PlaceDto placeDto)
    {
        var place = await _context.Places.FindAsync(id);
        if (place == null)
        {
            return ServiceResult<PlaceDto>.NotFound("Nie znaleziono miejsca");
        }
        place.Name = placeDto.Name;
        place.IsStorage = placeDto.IsStorage;
        await _context.SaveChangesAsync();
        return new ServiceResult<PlaceDto>(ServiceStatus.Ok, string.Empty, new PlaceDto(place));
    }

    public async Task<ServiceResult> DeletePlace(short id)
    {
        var place = await _context.Places.FindAsync(id);
        if (place == null)
        {
            return ServiceResult.NotFound("Nie znaleziono miejsca");
        }
        _context.Places.Remove(place);
        await _context.SaveChangesAsync();
        return new ServiceResult(ServiceStatus.Ok, string.Empty);
    }
}