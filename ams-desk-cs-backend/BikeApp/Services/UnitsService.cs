using ams_desk_cs_backend.BikeApp.Data;
using ams_desk_cs_backend.BikeApp.Dtos.Repairs;
using ams_desk_cs_backend.BikeApp.Interfaces;
using ams_desk_cs_backend.Shared.Results;
using Microsoft.EntityFrameworkCore;

namespace ams_desk_cs_backend.BikeApp.Services
{
    public class UnitsService : IUnitsService
    {
        private readonly BikesDbContext _context;
        public UnitsService(BikesDbContext context)
        {
            _context = context;
        }
        public async Task<ServiceResult<IEnumerable<UnitDto>>> GetUnits()
        {
            var result = await _context.Units.Select(unit => new UnitDto { Id = unit.UnitId, Name = unit.UnitName }).ToListAsync();
            return new ServiceResult<IEnumerable<UnitDto>>(ServiceStatus.Ok, string.Empty ,result);
        }
    }
}
