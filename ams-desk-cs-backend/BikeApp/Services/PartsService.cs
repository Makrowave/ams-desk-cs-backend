using ams_desk_cs_backend.BikeApp.Data;
using ams_desk_cs_backend.BikeApp.Data.Models.Repairs;
using ams_desk_cs_backend.BikeApp.Interfaces;
using ams_desk_cs_backend.Shared.Results;
using Microsoft.EntityFrameworkCore;

namespace ams_desk_cs_backend.BikeApp.Services
{
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
                .ThenByDescending(part => part.PartName)
                .Select(part => new Part
                {
                    PartId = part.PartId,
                    PartTypeId = part.PartTypeId,
                    PartName = part.PartName,
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
                .ThenByDescending(part => part.PartName)
                .Select(part => new Part
                {
                    PartId = part.PartId,
                    PartTypeId = part.PartTypeId,
                    PartName = part.PartName,
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
            existingPart.PartName = part.PartName;
            await _context.SaveChangesAsync();
            return new ServiceResult<Part>(ServiceStatus.Ok, string.Empty, part);
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
}
