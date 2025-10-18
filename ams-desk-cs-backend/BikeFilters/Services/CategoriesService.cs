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
        var categories = await _context.Categories.OrderBy(category => category.Order)
            .Select(category => new CategoryDto
            {
                Id = category.Id,
                Name = category.Name,
            }).ToListAsync();
        return new ServiceResult<IEnumerable<CategoryDto>>(ServiceStatus.Ok, string.Empty, categories);
    }

    public async Task<ServiceResult<CategoryDto>> PostCategory(CategoryDto categoryDto)
    {
        var order = _context.Categories.Count() + 1;
        var category = new Category
        {
            Name = categoryDto.Name,
            Order = (short)order
        };
        _context.Add(category);
        await _context.SaveChangesAsync();
        var result = new CategoryDto()
        {
            Id = category.Id,
            Name = category.Name,
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

        oldCategory.Name = newCategory.Name;
        await _context.SaveChangesAsync();
        var result = new CategoryDto
        {
            Id = oldCategory.Id,
            Name = oldCategory.Name,
        };
        return new ServiceResult<CategoryDto>(ServiceStatus.Ok, string.Empty, result);

    }
    public async Task<ServiceResult<List<CategoryDto>>> ChangeOrder(short source, short dest)
    {
        if (!_context.Categories.Any(c => c.Id == source || c.Id == dest))
        {
            return ServiceResult<List<CategoryDto>>.NotFound("Nie znaleziono zamienianych elementów");
        }
        
        var categories = await _context.Categories.OrderBy(c => c.Order).ToListAsync();
        
        var firstCategory = categories.First(c => c.Id == source);
        var lastCategory = categories.First(c => c.Id == dest);
        var firstOrder = firstCategory.Order;
        var lastOrder = lastCategory.Order;

        if (firstOrder < lastOrder)
        {
            var toShift = categories
                .Where(c => c.Order > firstOrder && c.Order <= lastOrder)
                .ToList();

            toShift.ForEach(c => c.Order--);
            firstCategory.Order = lastOrder;
        }
        else if (firstOrder > lastOrder)
        {
            var toShift = categories
                .Where(c => c.Order >= lastOrder && c.Order < firstOrder)
                .ToList();

            toShift.ForEach(c => c.Order++);
            firstCategory.Order = lastOrder;
        }

        await _context.SaveChangesAsync();

        var result = await _context.Categories
            .OrderBy(c => c.Order)
            .Select(c => new CategoryDto
            {
                Id = c.Id,
                Name = c.Name,
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