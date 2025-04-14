using ams_desk_cs_backend.Data.Models.Repairs;
using ams_desk_cs_backend.Repairs.Dtos;
using ams_desk_cs_backend.Shared.Results;

namespace ams_desk_cs_backend.Repairs.Interfaces;

public interface IServicesService
{
    public Task<ServiceResult<IEnumerable<Service>>> GetServices();
    public Task<ServiceResult<IEnumerable<Service>>> GetServicesFromCategory(short categoryId);
    public Task<ServiceResult<IEnumerable<ServiceCategoryDto>>> GetServiceCategories();
    public Task<ServiceResult<Service>> PutService(short id, Service service);
    public Task<ServiceResult> DeleteService(short id);
    public Task<ServiceResult<Service>> PostService(Service service);
}