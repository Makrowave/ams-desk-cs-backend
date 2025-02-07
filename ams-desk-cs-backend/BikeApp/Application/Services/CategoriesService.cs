using ams_desk_cs_backend.BikeApp.Application.Interfaces;
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
        public CategoriesService(BikesDbContext bikesDbContext) 
        {
            _context = bikesDbContext;
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
            var order = _context.Categories.Count() + 1;
            _context.Add(new Category
            {
                CategoryName = category.CategoryName,
                CategoriesOrder = (short)order
            });
            await _context.SaveChangesAsync();
            return new ServiceResult(ServiceStatus.Ok, string.Empty);
        }

        public async Task<ServiceResult> UpdateCategory(short id, CategoryDto newCategory)
        {
            var oldCategory = await _context.Categories.FindAsync(id);
            if (oldCategory == null)
            {
                return new ServiceResult(ServiceStatus.NotFound, "Nie znaleziono kategorii");
            }

            oldCategory.CategoryName = newCategory.CategoryName;
            await _context.SaveChangesAsync();
            return new ServiceResult(ServiceStatus.Ok, string.Empty);
        
        }
        public async Task<ServiceResult> ChangeOrder(short firstId, short lastId)
        {
            if (!_context.Categories.Any(category => (category.CategoryId == firstId || category.CategoryId == lastId)))
            {
                return new ServiceResult(ServiceStatus.NotFound, "Nie znaleziono zamienianych elementów");
            }
            var categories = await _context.Categories.OrderBy(category => category.CategoriesOrder).ToListAsync();
            var firstOrder = categories.FirstOrDefault(category => category.CategoryId == firstId)!.CategoriesOrder;
            var lastOrder = categories.FirstOrDefault(category => category.CategoryId == lastId)!.CategoriesOrder;

            var filteredCategories = 
                categories.Where(category => category.CategoriesOrder >= firstOrder && category.CategoriesOrder <= lastOrder).ToList();
            filteredCategories.ForEach(category => category.CategoriesOrder++);
            filteredCategories.Last().CategoriesOrder = firstOrder;
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
