using ams_desk_cs_backend.BikeApp.Application.Interfaces;
using ams_desk_cs_backend.BikeApp.Dtos.AppModelDto;
using ams_desk_cs_backend.BikeApp.Infrastructure.Data;
using ams_desk_cs_backend.Shared.Results;
using Microsoft.EntityFrameworkCore;

namespace ams_desk_cs_backend.BikeApp.Application.Services
{
    public class CategoriesService : ICategoriesService
    {
        private readonly BikesDbContext _context;
        public CategoriesService(BikesDbContext bikesDbContext) 
        {
            _context = bikesDbContext;
        }
        public async Task<ServiceResult<IEnumerable<CategoryDto>>> GetCategories()
        {
            var categories = await _context.Categories.Select(category => new CategoryDto
            {
                CategoryId = category.CategoryId,
                CategoryName = category.CategoryName,
            }).ToListAsync();
            return new ServiceResult<IEnumerable<CategoryDto>>(ServiceStatus.Ok, string.Empty, categories);
        }
    }
}
