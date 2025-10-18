using ams_desk_cs_backend.BikeFilters.Dtos;
using ams_desk_cs_backend.BikeFilters.Interfaces;
using ams_desk_cs_backend.Data;
using ams_desk_cs_backend.Data.Models;
using ams_desk_cs_backend.Shared.Results;
using Microsoft.EntityFrameworkCore;

namespace ams_desk_cs_backend.BikeFilters.Services;

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
                Id = category.CategoryId,
                Name = category.CategoryName,
            }).ToListAsync();
        return new ServiceResult<IEnumerable<CategoryDto>>(ServiceStatus.Ok, string.Empty, categories);
    }

    public async Task<ServiceResult<CategoryDto>> PostCategory(CategoryDto categoryDto)
    {
        var order = _context.Categories.Count() + 1;
        var category = new Category
        {
            CategoryName = categoryDto.Name,
            CategoriesOrder = (short)order
        };
        _context.Add(category);
        await _context.SaveChangesAsync();
        var result = new CategoryDto()
        {
            Id = category.CategoryId,
            Name = category.CategoryName,
        };
        return new ServiceResult<CategoryDto>(ServiceStatus.Ok, string.Empty, result);
    }

    public async Task<ServiceResult<CategoryDto>> UpdateCategory(short id, CategoryDto newCategory)
    {
        var oldCategory = await _context.Categories.FindAsync(id);
        if (oldCategory == null)
        {
            return ServiceResult<CategoryDto>.NotFound("Nie znaleziono kategorii");
        }

        oldCategory.CategoryName = newCategory.Name;
        await _context.SaveChangesAsync();
        var result = new CategoryDto
        {
            Id = oldCategory.CategoryId,
            Name = oldCategory.CategoryName,
        };
        return new ServiceResult<CategoryDto>(ServiceStatus.Ok, string.Empty, result);

    }
    public async Task<ServiceResult<List<CategoryDto>>> ChangeOrder(short source, short dest)
    {
        if (!_context.Categories.Any(c => c.CategoryId == source || c.CategoryId == dest))
        {
            return ServiceResult<List<CategoryDto>>.NotFound("Nie znaleziono zamienianych elementów");
        }
        
        var categories = await _context.Categories.OrderBy(c => c.CategoriesOrder).ToListAsync();
        
        var firstCategory = categories.First(c => c.CategoryId == source);
        var lastCategory = categories.First(c => c.CategoryId == dest);
        var firstOrder = firstCategory.CategoriesOrder;
        var lastOrder = lastCategory.CategoriesOrder;

        if (firstOrder < lastOrder)
        {
            var toShift = categories
                .Where(c => c.CategoriesOrder > firstOrder && c.CategoriesOrder <= lastOrder)
                .ToList();

            toShift.ForEach(c => c.CategoriesOrder--);
            firstCategory.CategoriesOrder = lastOrder;
        }
        else if (firstOrder > lastOrder)
        {
            var toShift = categories
                .Where(c => c.CategoriesOrder >= lastOrder && c.CategoriesOrder < firstOrder)
                .ToList();

            toShift.ForEach(c => c.CategoriesOrder++);
            firstCategory.CategoriesOrder = lastOrder;
        }

        await _context.SaveChangesAsync();

        var result = await _context.Categories
            .OrderBy(c => c.CategoriesOrder)
            .Select(c => new CategoryDto
            {
                Id = c.CategoryId,
                Name = c.CategoryName,
            }).ToListAsync();

        return new ServiceResult<List<CategoryDto>>(ServiceStatus.Ok, string.Empty, result);
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