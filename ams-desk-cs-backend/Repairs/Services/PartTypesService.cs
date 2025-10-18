using ams_desk_cs_backend.Data;
using ams_desk_cs_backend.Repairs.Dtos;
using ams_desk_cs_backend.Repairs.Interfaces;
using ams_desk_cs_backend.Shared.Results;
using Microsoft.EntityFrameworkCore;

namespace ams_desk_cs_backend.Repairs.Services;

public class PartTypesService : IPartTypesService
{
    private readonly BikesDbContext _context;

    public PartTypesService(BikesDbContext context)
    {
        _context = context;
    }

    public async Task<ServiceResult<IEnumerable<PartTypeDto>>> GetPartTypes(short id)
    {
        var result = await _context.PartTypes.Where(type => type.PartCategoryId == id || id == 0)
            .Select(type =>
                new PartTypeDto
                {
                    Id = type.Id,
                    Name = type.Name
                }).ToListAsync();
        return new ServiceResult<IEnumerable<PartTypeDto>>(ServiceStatus.Ok, string.Empty, result);
    }

    public async Task<ServiceResult<IEnumerable<PartCategoryDto>>> GetPartCategories()
    {
        var result = await _context.PartCategories.Select(category => new PartCategoryDto
        {
            Id = category.Id,
            Name = category.Name
        }).ToListAsync();
        return new ServiceResult<IEnumerable<PartCategoryDto>>(ServiceStatus.Ok, string.Empty, result);
    }
}