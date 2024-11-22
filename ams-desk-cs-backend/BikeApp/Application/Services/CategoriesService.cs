using ams_desk_cs_backend.BikeApp.Application.Interfaces;
using ams_desk_cs_backend.BikeApp.Application.Interfaces.Validators;
using ams_desk_cs_backend.BikeApp.Dtos.AppModelDto;
using ams_desk_cs_backend.BikeApp.Infrastructure.Data;
using ams_desk_cs_backend.BikeApp.Infrastructure.Data.Models;
using ams_desk_cs_backend.Shared.Results;
using Microsoft.EntityFrameworkCore;

namespace ams_desk_cs_backend.BikeApp.Application.Services
{
    public class CategoriesService : ICategoriesService
    {
        private readonly BikesDbContext _context;
        private readonly ICommonValidator _commonValidator;
        public CategoriesService(BikesDbContext bikesDbContext, ICommonValidator commonValidator) 
        {
            _context = bikesDbContext;
            _commonValidator = commonValidator;
        }
        public async Task<ServiceResult<IEnumerable<CategoryDto>>> GetCategories()
        {
            var categories = await _context.Categories.OrderBy(category => category.CategoriesOrder)
                .Select(category => new CategoryDto
            {
                CategoryId = category.CategoryId,
                CategoryName = category.CategoryName,
            }).ToListAsync();
            return new ServiceResult<IEnumerable<CategoryDto>>(ServiceStatus.Ok, string.Empty, categories);
        }

        public async Task<ServiceResult> PostCategory(CategoryDto category)
        {
            if (category.CategoryName == null || !_commonValidator.Validate16CharName(category.CategoryName))
            {
                return new ServiceResult(ServiceStatus.BadRequest, "Zła nazwa kategorii");
            }
            var order = _context.Categories.Count() + 1;
            _context.Add(new Category
            {
                CategoryName = category.CategoryName,
                CategoriesOrder = (short)order
            });
            await _context.SaveChangesAsync();
            return new ServiceResult(ServiceStatus.Ok, string.Empty);
        }

        public async Task<ServiceResult> UpdateCategory(short id, CategoryDto category)
        {
            var existingCategory = await _context.Categories.FindAsync(id);
            if (existingCategory == null)
            {
                return new ServiceResult(ServiceStatus.NotFound, "Nie znaleziono kategorii");
            }
            if (category.CategoryName != null && _commonValidator.Validate16CharName(category.CategoryName))
            {
                existingCategory.CategoryName = category.CategoryName;
            }
            await _context.SaveChangesAsync();
            return new ServiceResult(ServiceStatus.Ok, string.Empty);
        }
        public async Task<ServiceResult> DeleteCategory(short id)
        {
            try
            {
                var existingCategory = await _context.Categories.FindAsync(id);
                if (existingCategory == null)
                {
                    return new ServiceResult(ServiceStatus.NotFound, "Nie znaleziono kategorii");
                }
                _context.Categories.Remove(existingCategory);
                await _context.SaveChangesAsync();
                return new ServiceResult(ServiceStatus.Ok, string.Empty);
            }
            catch (Exception ex)
            {
                return new ServiceResult(ServiceStatus.BadRequest, "Kategoria jest przypisana do modelu");
            }
        }
    }
}
