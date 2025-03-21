using ams_desk_cs_backend.BikeApp.Data;
using ams_desk_cs_backend.BikeApp.Dtos.Repairs;
using ams_desk_cs_backend.BikeApp.Interfaces;
using ams_desk_cs_backend.Shared.Results;
using Microsoft.EntityFrameworkCore;

namespace ams_desk_cs_backend.BikeApp.Services
{
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
                        Id = type.PartTypeId,
                        Name = type.PartTypeName
                    }).ToListAsync();
            return new ServiceResult<IEnumerable<PartTypeDto>>(ServiceStatus.Ok, string.Empty, result);
        }

        public async Task<ServiceResult<IEnumerable<PartCategoryDto>>> GetPartCategories()
        {
            var result = await _context.PartCategories.Select(category => new PartCategoryDto
            {
                Id = category.PartCategoryId,
                Name = category.PartCategoryName
            }).ToListAsync();
            return new ServiceResult<IEnumerable<PartCategoryDto>>(ServiceStatus.Ok, string.Empty, result);
        }
    }
}