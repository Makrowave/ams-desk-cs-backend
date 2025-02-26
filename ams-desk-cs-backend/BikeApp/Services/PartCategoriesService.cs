using ams_desk_cs_backend.BikeApp.Data;
using ams_desk_cs_backend.BikeApp.Dtos.Repairs;
using ams_desk_cs_backend.BikeApp.Interfaces;
using ams_desk_cs_backend.Shared.Results;
using Microsoft.EntityFrameworkCore;

namespace ams_desk_cs_backend.BikeApp.Services
{
    public class PartCategoriesService : IPartCategoriesService
    {
        private readonly BikesDbContext _context;
        public PartCategoriesService(BikesDbContext context)
        {
            _context = context;
        }
        public async Task<ServiceResult<IEnumerable<PartCategoryDto>>> GetPartCategories()
        {
            var result = await _context.PartCategories.Select(category => new PartCategoryDto { Id = category.PartCategoryId, Name = category.PartCategoryName}).ToListAsync();
            return new ServiceResult<IEnumerable<PartCategoryDto>>(ServiceStatus.Ok, string.Empty, result);
        }
    }
}
