using ams_desk_cs_backend.BikeApp.Data;
using ams_desk_cs_backend.BikeApp.Data.Models.Repairs;
using ams_desk_cs_backend.BikeApp.Interfaces;
using ams_desk_cs_backend.Shared.Results;
using Microsoft.EntityFrameworkCore;

namespace ams_desk_cs_backend.BikeApp.Services
{
    public class ServicesService : IServicesService
    {
        private readonly BikesDbContext _context;
        public ServicesService(BikesDbContext context)
        {
            _context = context;
        }


        public async Task<ServiceResult<IEnumerable<Service>>> GetServices()
        {
            var services = await _context.Services
                .Include(service => service.ServicesDone)
                .OrderByDescending(service => service.ServicesDone.Count())
                .Select(service => new Service
                {
                    ServiceId = service.ServiceId,
                    ServiceCategoryId = service.ServiceCategoryId,
                    ServiceName = service.ServiceName,
                    Price = service.Price,
                }).ToListAsync();

            return new ServiceResult<IEnumerable<Service>>(ServiceStatus.Ok, string.Empty, services);
        }
    }
}
