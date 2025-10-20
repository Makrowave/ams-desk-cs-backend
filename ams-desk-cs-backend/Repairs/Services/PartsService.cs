using ams_desk_cs_backend.Data;
using ams_desk_cs_backend.Data.Models.Repairs;
using ams_desk_cs_backend.Repairs.Interfaces;
using ams_desk_cs_backend.Shared.Results;
using Microsoft.EntityFrameworkCore;

namespace ams_desk_cs_backend.Repairs.Services;

public class PartsService : IPartsService
{
    private readonly BikesDbContext _context;
    public PartsService(BikesDbContext context)
    {
        _context = context;
    }

    public async Task<ServiceResult<IEnumerable<Part>>> GetFilteredParts(short categoryId, short typeId)
    {
        var parts= await _context.Parts.Include(part => part.PartsUsed)
            .Include(part => part.Unit)
            .Include(part => part.PartType)
            .ThenInclude(partType => partType!.PartCategory)
            .Where(part => part.PartTypeId == typeId || typeId == 0)
            .Where(part => part.PartType!.PartCategoryId == categoryId || categoryId == 0)
            .OrderByDescending(part => part.PartsUsed.Count)
            .ThenByDescending(part => part.Name)
            .Select(part => new Part
            {
                Id = part.Id,
                PartTypeId = part.PartTypeId,
                Name = part.Name,
                Price = part.Price,
                UnitId = part.UnitId,
                Unit = part.Unit,
                PartType = part.PartType
            })
            .ToListAsync();
        parts.ForEach(part => part.PartType!.Parts = []);
        parts.ForEach(part => part.PartType!.PartCategory!.PartTypes = []);
        return new ServiceResult<IEnumerable<Part>>(ServiceStatus.Ok, string.Empty, parts);
    }

    public async Task<ServiceResult<Part>> AddPart(Part part)
    {
        _context.Parts.Add(part);
        await _context.SaveChangesAsync();
        return new ServiceResult<Part>(ServiceStatus.Ok, string.Empty, part);
    }

    public async Task<ServiceResult<IEnumerable<Part>>> GetParts()
    {
        var parts= await _context.Parts.Include(part => part.PartsUsed)
            .Include(part => part.Unit)
            .Include(part => part.PartType)
            .OrderByDescending(part => part.PartsUsed.Count)
            .ThenByDescending(part => part.Name)
            .Select(part => new Part
            {
                Id = part.Id,
                PartTypeId = part.PartTypeId,
                Name = part.Name,
                Price = part.Price,
                UnitId = part.UnitId,
                Unit = part.Unit,
                PartType = part.PartType
            })
            .ToListAsync();
        parts.ForEach(part => part.PartType!.Parts = []);
        return new ServiceResult<IEnumerable<Part>>(ServiceStatus.Ok, string.Empty, parts);
    }

    public async Task<ServiceResult<Part>> ChangePart(int partId, Part part)
    {
        var existingPart = await _context.Parts.FindAsync(partId);
        if (existingPart == null)
        {
            return ServiceResult<Part>.NotFound("Nie znaleziono części");
        }
        existingPart.PartTypeId = part.PartTypeId;
        existingPart.UnitId = part.UnitId;
        existingPart.Price = part.Price;
        existingPart.Name = part.Name;
        await _context.SaveChangesAsync();
        var result = await _context.Parts.Include(p => p.PartsUsed)
            .Include(p => p.Unit)
            .Include(p => p.PartType)
            .FirstOrDefaultAsync(p => p.Id == partId);
        return new ServiceResult<Part>(ServiceStatus.Ok, string.Empty, result);
    }

    public async Task<ServiceResult<Dictionary<string, object>>> MergeParts(int id1, int id2, Part part)
    {
        var part1 = await _context.Parts.FindAsync(id1);
        var part2 = await _context.Parts.FindAsync(id2);
        if (part1 == null || part2 == null)
        {
            return ServiceResult<Dictionary<string, object>>.NotFound("Nie znaleziono jednej z części");
        }
        part1.Name = part.Name;
        part1.UnitId = part.UnitId;
        part1.Price = part.Price;
        part1.PartTypeId = part.PartTypeId;
        var partsUsed = await _context.PartsUsed.Where(partUsed => partUsed.PartId == part2.Id).ToListAsync();
        partsUsed.ForEach(partUsed => partUsed.PartId = part1.Id);
        _context.Remove(part2);
        await _context.SaveChangesAsync();
        var newPart = await _context.Parts.Include(p => p.PartsUsed)
            .Include(p => p.Unit)
            .Include(p => p.PartType)
            .FirstOrDefaultAsync(p => p.Id == id1);
        var result = new Dictionary<string, object>();
        result.Add("keptId", newPart!.Id);
        result.Add("removedId", part2.Id);
        result.Add("part", newPart);
        return new ServiceResult<Dictionary<string, object>>(ServiceStatus.Ok, string.Empty, result);
    }

    public async Task<ServiceResult> DeletePart(int id)
    {
        var part = await _context.Parts.FindAsync(id);
        if (part == null)
        {
            return ServiceResult<Part>.NotFound("Nie znaleziono części");
        }
        _context.Parts.Remove(part);
        await _context.SaveChangesAsync();
        return new ServiceResult(ServiceStatus.Ok, string.Empty);
    }
}