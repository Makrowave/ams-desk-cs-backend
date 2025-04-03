using ams_desk_cs_backend.BikeApp.Data;
using ams_desk_cs_backend.BikeApp.Data.Models.Repairs;
using ams_desk_cs_backend.BikeApp.Dtos.AppModelDto;
using ams_desk_cs_backend.BikeApp.Dtos.Repairs;
using ams_desk_cs_backend.BikeApp.Interfaces;
using ams_desk_cs_backend.Shared.Results;
using Microsoft.EntityFrameworkCore;

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
                StatusId = (short)RepairStatuses.Pending,
                PlaceId = newRepair.PlaceId,
                TakeInEmployeeId = newRepair.TakeInEmployeeId,
            };
            _context.Repairs.Add(repair);
            await _context.SaveChangesAsync();
            var id = repair.RepairId;
            return new ServiceResult<int>(ServiceStatus.Ok, string.Empty, id);
        }

        public async Task<ServiceResult<RepairDto>> GetRepair(int id)
        {
            var repair = await GetRepairFromDbAsync(id);
            if (repair == null)
            {
                return new ServiceResult<RepairDto>(ServiceStatus.NotFound, "Nie znaleziono zgłoszenia.", null);
            }

            var repairDto = new RepairDto(repair);
            return new ServiceResult<RepairDto>(ServiceStatus.Ok, string.Empty, repairDto);
        }

        public async Task<ServiceResult<IEnumerable<ShortRepairDto>>> GetRepairs(short place, short[] excludedStatuses)
        {
            var repairs = await _context.Repairs
                .Where(repair => repair.PlaceId == place || place == 0)
                .Where(repair => !excludedStatuses.Any(status => status == (short)repair.StatusId))
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


        //Does not update statusId, collectionEmployeeId, repairEmployeeId and dates
        public async Task<ServiceResult<RepairDto>> UpdateRepair(int id, RepairDto newRepair)
        {
            var oldRepair = await GetRepairFromDbAsync(id);
            if (oldRepair == null)
            {
                return new ServiceResult<RepairDto>(ServiceStatus.NotFound, "Nie znaleziono zgłoszenia.", null);
            }

            oldRepair.PhoneNumber = newRepair.PhoneNumber;
            oldRepair.BikeName = newRepair.BikeName;
            oldRepair.Issue = newRepair.Issue;
            oldRepair.Discount = newRepair.Discount;
            oldRepair.AdditionalCosts = newRepair.AdditionalCosts;
            oldRepair.PlaceId = newRepair.PlaceId;
            oldRepair.Note = newRepair.Note;

            //Handle new services:
            //Select services that are in the old one but are not in new one
            var deletedServices = oldRepair.Services.Where(service =>
                !newRepair.Services.Select(s => s.ServiceDoneId).Contains(service.ServiceDoneId));
            var addedServices = newRepair.Services.Where(s => s.ServiceDoneId == 0);
            _context.ServicesDone.RemoveRange(deletedServices);
            _context.ServicesDone.AddRange(addedServices);

            //Handle new parts:
            //Same as services
            var deletedParts = oldRepair.Parts.Where(part =>
                !newRepair.Parts.Select(p => p.PartUsedId).Contains(part.PartUsedId));
            var addedParts = newRepair.Parts.Where(p => p.PartUsedId == 0);
            _context.PartsUsed.RemoveRange(deletedParts);
            _context.PartsUsed.AddRange(addedParts);


            await _context.SaveChangesAsync();
            var result = new RepairDto((await GetRepairFromDbAsync(id))!);
            return new ServiceResult<RepairDto>(ServiceStatus.Ok, string.Empty, result);
        }

        public async Task<ServiceResult<RepairDto>> UpdateStatus(int id, short statusId)
        {
            var oldRepair = await GetRepairFromDbAsync(id);
            if (oldRepair == null)
            {
                return new ServiceResult<RepairDto>(ServiceStatus.NotFound,
                    "Nie znaleziono zgłoszenia.", null);
            }

            if ((RepairStatuses)statusId == RepairStatuses.Pending)
            {
                return new ServiceResult<RepairDto>(ServiceStatus.BadRequest,
                    "Nie można zmienić statusu na oczekujący", null);
            }

            if (oldRepair.StatusId == (short)RepairStatuses.Collected)
            {
                return new ServiceResult<RepairDto>(ServiceStatus.BadRequest,
                    "Nie można edytować ukończonego zgłoszenia", null);
            }

            if (await _context.RepairStatuses.FindAsync(statusId) == null)
            {
                return new ServiceResult<RepairDto>(ServiceStatus.NotFound,
                    "Nie znaleziono statusu.", null);
            }

            if ((RepairStatuses)statusId == RepairStatuses.Collected)
            {
                oldRepair.CollectionDate = DateOnly.FromDateTime(DateTime.Now);
            }

            oldRepair.StatusId = statusId;
            await _context.SaveChangesAsync(true);
            oldRepair.Status = await _context.RepairStatuses.FindAsync(oldRepair.StatusId);
            var result = new RepairDto(oldRepair);
            return new ServiceResult<RepairDto>(ServiceStatus.Ok, string.Empty, result);
        }

        //When finishing repair - this has to be performed before status change
        //When starting repair - this has to be performed after status change
        public async Task<ServiceResult<RepairDto>> UpdateEmployee(int id, short employeeId, bool collection)
        {
            var oldRepair = await GetRepairFromDbAsync(id);
            if (oldRepair == null)
            {
                return new ServiceResult<RepairDto>(ServiceStatus.NotFound,
                    "Nie znaleziono zgłoszenia.", null);
            }

            if (await _context.Employees.FindAsync(employeeId) == null)
            {
                return new ServiceResult<RepairDto>(ServiceStatus.NotFound,
                    "Nie znaleziono pracownika.", null);
            }

            var status = oldRepair.StatusId;

            if (status == (short)RepairStatuses.Collected)
            {
                return new ServiceResult<RepairDto>(ServiceStatus.BadRequest,
                    "Nie można edytować ukończonego zgłoszenia", null);
            }

            if (status == (short)RepairStatuses.Pending)
            {
                return new ServiceResult<RepairDto>(ServiceStatus.BadRequest,
                    "Nie można zmienić pracownika przy nierozpoczętym zgłoszeniu", null);
            }

            await _context.SaveChangesAsync();
            var result = new RepairDto(oldRepair);
            return new ServiceResult<RepairDto>(ServiceStatus.Ok, string.Empty, result);
        }

        public async Task<ServiceResult<RepairDto>> StartRepair(int id, short employeeId)
        {
            var repair = await GetRepairFromDbAsync(id);
            if (repair == null)
            {
                return new ServiceResult<RepairDto>(ServiceStatus.NotFound,
                    "Nie znaleziono zgłoszenia.", null);
            }

            if (await _context.Employees.FindAsync(employeeId) == null)
            {
                return new ServiceResult<RepairDto>(ServiceStatus.NotFound,
                    "Nie znaleziono pracownika.", null);
            }
            var statusId = repair.StatusId;

            if (statusId != (short)RepairStatuses.Pending)
            {
                return ServiceResult<RepairDto>.BadRequest("Zgłoszenie jest już rozpoczęte");
            }
            repair.RepairEmployeeId = employeeId;
            repair.StatusId = (short)RepairStatuses.InProgress;
            await _context.SaveChangesAsync();
            repair.Status = await _context.RepairStatuses.FindAsync(repair.StatusId);
            var result = new RepairDto(repair);
            return new ServiceResult<RepairDto>(ServiceStatus.Ok, string.Empty, result);
        }

        public async Task<ServiceResult<RepairDto>> CollectRepair(int id, short employeeId)
        {
            var repair = await GetRepairFromDbAsync(id);
            if (repair == null)
            {
                return new ServiceResult<RepairDto>(ServiceStatus.NotFound,
                    "Nie znaleziono zgłoszenia.", null);
            }

            if (await _context.Employees.FindAsync(employeeId) == null)
            {
                return new ServiceResult<RepairDto>(ServiceStatus.NotFound,
                    "Nie znaleziono pracownika.", null);
            }
            var statusId = repair.StatusId;

            if (statusId == (short)RepairStatuses.Pending || statusId == (short)RepairStatuses.Collected)
            {
                return ServiceResult<RepairDto>.BadRequest("Zgłoszenie nie jest rozpoczęte lub jest zakończone");
            }
            repair.CollectionEmployeeId = employeeId;
            repair.StatusId = (short)RepairStatuses.Collected;
            repair.CollectionDate = DateOnly.FromDateTime(DateTime.Now);
            await _context.SaveChangesAsync();
            repair.Status = await _context.RepairStatuses.FindAsync(repair.StatusId);
            var result = new RepairDto(repair);
            return new ServiceResult<RepairDto>(ServiceStatus.Ok, string.Empty, result);
        }

        private async Task<Repair?> GetRepairFromDbAsync(int id)
        {
            return await _context.Repairs.Where(repair => repair.RepairId == id)
                .Include(repair => repair.Parts)
                .ThenInclude(part => part.Part)
                .ThenInclude(part => part!.Unit)
                .Include(repair => repair.Services)
                .ThenInclude(service => service.Service)
                .Include(repair => repair.Status)
                .Include(repair => repair.CollectionEmployee)
                .Include(repair => repair.RepairEmployee)
                .Include(repair => repair.TakeInEmployee)
                .FirstOrDefaultAsync();
        }
    }
}