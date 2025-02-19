using ams_desk_cs_backend.BikeApp.Data;
using ams_desk_cs_backend.BikeApp.Data.Models;
using ams_desk_cs_backend.BikeApp.Data.Models.Repairs;
using ams_desk_cs_backend.BikeApp.Dtos.Repairs;
using ams_desk_cs_backend.BikeApp.Interfaces;
using ams_desk_cs_backend.Migrations;
using ams_desk_cs_backend.Shared.Results;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections;
using System.IO;
using System.Linq;

namespace ams_desk_cs_backend.BikeApp.Services
{
    public class RepairsService : IRepairsService
    {
        private readonly BikesDbContext _context;
        public RepairsService(BikesDbContext context)
        {
            _context = context;
        }
        public async Task<ServiceResult<int>> CreateRepair(NewRepairDto newRepair)
        {
            var repair = new Repair
            {
                PhoneNumber = newRepair.PhoneNumber,
                BikeName = newRepair.BikeName,
                Issue = newRepair.Issue,
                ArrivalDate = DateOnly.FromDateTime(DateTime.Now),
                StatusId = 1,
                PlaceId = newRepair.PlaceId,
            };
            _context.Repairs.Add(repair);
            await _context.SaveChangesAsync();
            var id = repair.RepairId;
            return new ServiceResult<int>(ServiceStatus.Ok, string.Empty, id);
        }

        public async Task<ServiceResult<RepairDto>> GetRepair(int id)
        {
            var repair = await _context.Repairs.Where(repair => repair.RepairId == id)
                .Include(repair => repair.Parts)
                .ThenInclude(part => part.Part)
                .ThenInclude(part => part!.Unit)
                .Include(repair => repair.Services)
                .ThenInclude(service => service.Service)
                .Include(repair => repair.Status)
                .Include(repair => repair.CollectionEmployee)
                .Include(repair => repair.RepairEmployee)
                .FirstOrDefaultAsync();
            if (repair == null)
            {
                return new ServiceResult<RepairDto>(ServiceStatus.NotFound, "Nie znaleziono zgłoszenia.", null);
            }
            var repairDto = new RepairDto
            {
                RepairId = id,
                PhoneNumber = repair.PhoneNumber,
                BikeName = repair.BikeName,
                Issue = repair.Issue,
                ArrivalDate = repair.ArrivalDate,
                CollectionDate = repair.CollectionDate,
                RepairEmployeeId = repair.RepairEmployeeId,
                CollectionEmployeeId = repair.CollectionEmployeeId,
                Discount = repair.Discount,
                AdditionalCosts = repair.AdditionalCosts,
                StatusId = repair.StatusId,
                PlaceId = repair.PlaceId,
                Note = repair.Note,
                Services = repair.Services,
                Parts = repair.Parts,
                Status = repair.Status,
                RepairEmployeeName = repair.RepairEmployee?.EmployeeName,
                CollectionEmployeeName = repair.CollectionEmployee?.EmployeeName

            };
            return new ServiceResult<RepairDto>(ServiceStatus.Ok, string.Empty, repairDto);
        }

        public async Task<ServiceResult<IEnumerable<ShortRepairDto>>> GetRepairs(short place, short[] excludedStatuses)
        {

            var repairs = await _context.Repairs
                .Where(repair => repair.PlaceId == place || place == 0)
                .Where(repair => !excludedStatuses.Any(status => status == repair.StatusId))
                .Include(repair => repair.Place)
                .Include(repair => repair.Status)
                .Select(
                (repair) => new ShortRepairDto
                {
                    Id = repair.RepairId,
                    PhoneNumber = repair.PhoneNumber,
                    BikeName = repair.BikeName,
                    Date = repair.ArrivalDate,
                    Status = repair.Status!,
                    PlaceId = repair.PlaceId,
                    PlaceName = repair.Place!.PlaceName,
                }).OrderByDescending(repair => repair.Id).ToListAsync();
            return new ServiceResult<IEnumerable<ShortRepairDto>>(ServiceStatus.Ok, string.Empty, repairs);
        }


        public async Task<ServiceResult> UpdateRepair(int id, RepairDto newRepair)
        {
            var oldRepair = await _context.Repairs.Where(repair => repair.RepairId == id)
                .Include(repair => repair.Parts)
                .Include(repair => repair.Services)
                .FirstOrDefaultAsync();
            if (oldRepair == null)
            {
                return new ServiceResult(ServiceStatus.NotFound, "Nie znaleziono zgłoszenia.");
            }
            oldRepair.PhoneNumber = newRepair.PhoneNumber;
            oldRepair.BikeName = newRepair.BikeName;
            oldRepair.Issue = newRepair.Issue;
            oldRepair.ArrivalDate = newRepair.ArrivalDate;
            oldRepair.CollectionDate = newRepair.CollectionDate;
            oldRepair.RepairEmployeeId = newRepair.RepairEmployeeId;
            oldRepair.CollectionEmployeeId = newRepair.CollectionEmployeeId;
            oldRepair.Discount = newRepair.Discount;
            oldRepair.AdditionalCosts = newRepair.AdditionalCosts;
            oldRepair.StatusId = newRepair.StatusId;
            oldRepair.PlaceId = newRepair.PlaceId;
            oldRepair.Note = newRepair.Note;

            //Handle new services:
            var deletedServices = oldRepair.Services.Except(newRepair.Services);
            var addedServices = newRepair.Services.Except(oldRepair.Services);
            _context.ServicesDone.RemoveRange(deletedServices);
            foreach(var service in addedServices)
            {
                service.ServiceDoneId = 0;
            }
            _context.ServicesDone.AddRange(addedServices);

            //Handle new parts:
            var deletedParts = oldRepair.Parts.Except(newRepair.Parts);
            var addedParts = newRepair.Parts.Except(oldRepair.Parts);
            _context.PartsUsed.RemoveRange(deletedParts);
            foreach (var part in addedParts)
            {
                part.PartUsedId = 0;
            }
            _context.PartsUsed.AddRange(addedParts);


            await _context.SaveChangesAsync();
            return new ServiceResult(ServiceStatus.Ok, string.Empty);
        }
    }
}
