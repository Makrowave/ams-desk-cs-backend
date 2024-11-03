using ams_desk_cs_backend.BikeApp.Application.Interfaces;
using ams_desk_cs_backend.BikeApp.Application.Results;
using ams_desk_cs_backend.BikeApp.Dtos.AppModelDto;
using ams_desk_cs_backend.BikeApp.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ams_desk_cs_backend.BikeApp.Application.Services
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
            var manufacturers = await _context.Manufacturers.Select(manufacter => new ManufacturerDto
            {
                ManufacturerId = manufacter.ManufacturerId,
                ManufacturerName = manufacter.ManufacturerName,

            }).ToListAsync();
            return new ServiceResult<IEnumerable<ManufacturerDto>>(ServiceStatus.Ok, string.Empty, manufacturers);
        }
    }
}
