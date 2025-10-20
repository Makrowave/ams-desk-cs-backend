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

    public async Task<ServiceResult<IEnumerable<PlaceDto>>> ChangeOrder(short firstId, short lastId)
    {
        if (!_context.Places.Any(place => place.Id == firstId || place.Id == lastId))
        {
            return ServiceResult<IEnumerable<PlaceDto>>.NotFound("Nie znaleziono zamienianych elementów");
        }
        var places = await _context.Places.OrderBy(place => place.Order).ToListAsync();
        var firstOrder = places.FirstOrDefault(place => place.Id == firstId)!.Order;
        var lastOrder = places.FirstOrDefault(place => place.Id == lastId)!.Order;

        var filteredPlaces = places.Where(place => place.Order >= firstOrder && place.Order <= lastOrder).ToList();
        filteredPlaces.ForEach(place => place.Order++);
        filteredPlaces.Last().Order = firstOrder;
        await _context.SaveChangesAsync();
        var result = await _context.Places.OrderBy(place => place.Order)
            .Select(place => new PlaceDto(place))
            .ToListAsync();
        return new ServiceResult<IEnumerable<PlaceDto>>(ServiceStatus.Ok, string.Empty, result);
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