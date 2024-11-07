using ams_desk_cs_backend.BikeApp.Application.Interfaces;
using ams_desk_cs_backend.BikeApp.Dtos.AppModelDto;
using ams_desk_cs_backend.BikeApp.Infrastructure.Data;
using ams_desk_cs_backend.Shared.Results;
using Microsoft.EntityFrameworkCore;

namespace ams_desk_cs_backend.BikeApp.Application.Services
{
    public class PlaceService : IPlacesService
    {
        private readonly BikesDbContext _context;
        public PlaceService(BikesDbContext context)
        {
            _context = context;
        }
        public async Task<ServiceResult<IEnumerable<PlaceDto>>> GetPlaces()
        {
            var places = await _context.Places.Select(place => new PlaceDto
            {
                PlaceId = place.PlaceId,
                PlaceName = place.PlaceName,
            }).ToListAsync();
            return new ServiceResult<IEnumerable<PlaceDto>>(ServiceStatus.Ok, string.Empty, places);
        }
    }
}
