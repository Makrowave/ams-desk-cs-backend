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

        public async Task<ServiceResult> AddPart(Part part)
        {
            _context.Parts.Add(part);
            await _context.SaveChangesAsync();
            return new ServiceResult(ServiceStatus.Ok, string.Empty);
        }

        public async Task<ServiceResult<IEnumerable<Part>>> GetParts()
        {
            var parts= await _context.Parts.Include(part => part.PartsUsed)
                .Include(part => part.Unit)
                .OrderByDescending(part => part.PartsUsed.Count)
                .ThenByDescending(part => part.PartName)
                .Select(part => new Part
                {
                    PartId = part.PartId,
                    PartCategoryId = part.PartCategoryId,
                    PartName = part.PartName,
                    Price = part.Price,
                    UnitId = part.UnitId,
                    Unit = part.Unit,
                })
                .ToListAsync();
            return new ServiceResult<IEnumerable<Part>>(ServiceStatus.Ok, string.Empty, parts);
        }
    }
}
