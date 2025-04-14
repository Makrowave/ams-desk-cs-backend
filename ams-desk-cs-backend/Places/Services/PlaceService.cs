using ams_desk_cs_backend.Data;
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
        var places = await _context.Places.OrderBy(place => place.PlacesOrder)
            .Select(place => new PlaceDto
            {
                PlaceId = place.PlaceId,
                PlaceName = place.PlaceName,
            }).ToListAsync();
        return new ServiceResult<IEnumerable<PlaceDto>>(ServiceStatus.Ok, string.Empty, places);
    }
    public async Task<ServiceResult> ChangeOrder(short firstId, short lastId)
    {
        if (!_context.Places.Any(place => place.PlaceId == firstId || place.PlaceId == lastId))
        {
            return new ServiceResult(ServiceStatus.NotFound, "Nie znaleziono zamienianych elementów");
        }
        var places = await _context.Places.OrderBy(place => place.PlacesOrder).ToListAsync();
        var firstOrder = places.FirstOrDefault(place => place.PlaceId == firstId)!.PlacesOrder;
        var lastOrder = places.FirstOrDefault(place => place.PlaceId == lastId)!.PlacesOrder;

        var filteredPlaces = places.Where(place => place.PlacesOrder >= firstOrder && place.PlacesOrder <= lastOrder).ToList();
        filteredPlaces.ForEach(place => place.PlacesOrder++);
        filteredPlaces.Last().PlacesOrder = firstOrder;
        await _context.SaveChangesAsync();
        return new ServiceResult(ServiceStatus.Ok, string.Empty);
    }
}