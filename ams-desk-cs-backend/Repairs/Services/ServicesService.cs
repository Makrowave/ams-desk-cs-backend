using ams_desk_cs_backend.Data;
using ams_desk_cs_backend.Repairs.Dtos;
using ams_desk_cs_backend.Repairs.Interfaces;
using ams_desk_cs_backend.Shared.Results;
using Microsoft.EntityFrameworkCore;
using Service = ams_desk_cs_backend.Data.Models.Repairs.Service;

namespace ams_desk_cs_backend.Repairs.Services;

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
            .Include(service => service.ServiceCategory)
            .OrderByDescending(service => service.ServicesDone.Count())
            .Select(service => new Service
            {
                ServiceId = service.ServiceId,
                ServiceCategoryId = service.ServiceCategoryId,
                ServiceName = service.ServiceName,
                Price = service.Price,
                ServiceCategory = service.ServiceCategory
            }).ToListAsync();
        services.ForEach(service => service.ServiceCategory!.Services = []);
        return new ServiceResult<IEnumerable<Service>>(ServiceStatus.Ok, string.Empty, services);
    }

    public async Task<ServiceResult<IEnumerable<Service>>> GetServicesFromCategory(short categoryId)
    {
        var services = await _context.Services
            .Include(service => service.ServicesDone)
            .Include(service => service.ServiceCategory)
            .Where(service => service.ServiceCategoryId == categoryId || categoryId == 0)
            .OrderByDescending(service => service.ServicesDone.Count())
            .Select(service => new Service
            {
                ServiceId = service.ServiceId,
                ServiceCategoryId = service.ServiceCategoryId,
                ServiceName = service.ServiceName,
                Price = service.Price,
                ServiceCategory = service.ServiceCategory
            }).ToListAsync();
        services.ForEach(service => service.ServiceCategory!.Services = []);
        return new ServiceResult<IEnumerable<Service>>(ServiceStatus.Ok, string.Empty, services);
    }

    public async Task<ServiceResult<IEnumerable<ServiceCategoryDto>>> GetServiceCategories()
    {
        var categories = await _context.ServiceCategories.Select(category => new ServiceCategoryDto
        {
            Id = category.ServiceCategoryId,
            Name = category.ServiceCategoryName
        }).ToListAsync();
        return new ServiceResult<IEnumerable<ServiceCategoryDto>>(ServiceStatus.Ok, string.Empty, categories);
    }

    public async Task<ServiceResult<Service>> PutService(short id, Service service)
    {
        var existingService = await _context.Services.FindAsync(id);
        if (existingService == null)
        {
            return ServiceResult<Service>.NotFound("Nie znaleziono usługi");
        }
        existingService.Price = service.Price;
        existingService.ServiceCategoryId = service.ServiceCategoryId;
        existingService.ServiceName = service.ServiceName;
        await _context.SaveChangesAsync();
        return new ServiceResult<Service>(ServiceStatus.Ok, string.Empty, existingService);
    }

    public async Task<ServiceResult> DeleteService(short id)
    {
        var service = await _context.Services.FindAsync(id);
        if (service == null)
        {
            return ServiceResult<Service>.NotFound("Nie znaleziono usługi");
        }
        _context.Services.Remove(service);
        await _context.SaveChangesAsync();
        return new ServiceResult(ServiceStatus.Ok, string.Empty);
    }

    public async Task<ServiceResult<Service>> PostService(Service service)
    {
        _context.Services.Add(service);
        await _context.SaveChangesAsync();
        return new ServiceResult<Service>(ServiceStatus.Ok, string.Empty, service);
    }
}