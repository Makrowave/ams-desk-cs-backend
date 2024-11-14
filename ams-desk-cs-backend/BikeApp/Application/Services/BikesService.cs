using ams_desk_cs_backend.BikeApp.Api.Dtos;
using ams_desk_cs_backend.BikeApp.Application.Interfaces;
using ams_desk_cs_backend.BikeApp.Dtos.AppModelDto;
using ams_desk_cs_backend.BikeApp.Infrastructure.Data;
using ams_desk_cs_backend.BikeApp.Infrastructure.Data.Models;
using ams_desk_cs_backend.BikeApp.Infrastructure.Enums;
using ams_desk_cs_backend.Shared.Results;
using Microsoft.EntityFrameworkCore;

namespace ams_desk_cs_backend.BikeApp.Application.Services
{
    public class BikesService : IBikesService
    {
        private readonly BikesDbContext _context;
        public BikesService(BikesDbContext dbContext)
        {
            _context = dbContext;
        }

        public async Task<ServiceResult> PutBike(int id, BikeDto bike)
        {
            var existingBike = await _context.Bikes.FindAsync(id);
            if (existingBike == null)
            {
                return new ServiceResult(ServiceStatus.NotFound, "Nie znaleziono roweru");
            }
            if(bike.ModelId.HasValue)
            {
                existingBike.ModelId = bike.ModelId.Value;
            }
            if (bike.PlaceId.HasValue)
            {
                existingBike.PlaceId = bike.PlaceId.Value;
            }
            if (bike.StatusId.HasValue)
            {
                existingBike.StatusId = bike.StatusId.Value;
                if(bike.StatusId.Value == (short) BikeStatus.Sold && !bike.SaleDate.HasValue)
                {
                    existingBike.SaleDate = DateOnly.FromDateTime(DateTime.Today);
                }
            }
            if (bike.InsertionDate.HasValue)
            {
                existingBike.InsertionDate = bike.InsertionDate.Value;
            }
            if (bike.SaleDate.HasValue)
            {
                existingBike.SaleDate = bike.SaleDate.Value;
            }
            if (bike.SalePrice.HasValue)
            {
                existingBike.SalePrice = bike.SalePrice.Value;
            }
            if (bike.AssembledBy.HasValue)
            {
                existingBike.AssembledBy = bike.AssembledBy.Value;
            }
            await _context.SaveChangesAsync();
            return new ServiceResult(ServiceStatus.Ok, String.Empty);
        }

        public async Task<ServiceResult<IEnumerable<BikeSubRecordDto>>> GetBikes(int modelId, short placeId)
        {
            if(_context.Models.Find(modelId) == null)
            {
                return new ServiceResult<IEnumerable<BikeSubRecordDto>>(ServiceStatus.NotFound, "Nie znaleziono modelu", null);
            }
            if(_context.Places.Find(placeId) == null && placeId != 0)
            {
                return new ServiceResult<IEnumerable<BikeSubRecordDto>>(ServiceStatus.NotFound, "Nie znaleziono miejsca", null);
            }
            var bikes = await _context.Bikes
                .Where(bi => bi.ModelId == modelId && bi.StatusId != (short)BikeStatus.Sold && (placeId == 0 || bi.PlaceId == placeId))
                .GroupJoin(
                    _context.Places,
                    bi => bi.PlaceId,
                    pl => pl.PlaceId,
                    (bi, pl) => new { bi, pl }
                )
                .SelectMany(
                    g => g.pl.DefaultIfEmpty(),
                    (g1, pl) => new { g1.bi, pl }
                )
                .GroupJoin(
                    _context.Statuses,
                    g => g.bi.StatusId,
                    st => st.StatusId,
                    (g, st) => new { g.bi, g.pl, st }
                )
                .SelectMany(
                    g => g.st.DefaultIfEmpty(),
                    (g, st) => new { g.bi, g.pl, st }
                )
                .OrderBy(g => g.st.StatusId).ThenBy(g => g.pl.PlaceId)
                .GroupJoin(
                    _context.Employees,
                    g => g.bi.AssembledBy,
                    emp => emp.EmployeeId,
                    (g, emp) => new { g.bi, g.pl, g.st, emp }
                )
                .SelectMany(
                    g => g.emp.DefaultIfEmpty(),
                    (g, emp) => new BikeSubRecordDto
                    {
                        Id = g.bi.BikeId,
                        Place = g.pl!.PlaceName,
                        Status = g.st!.StatusName,
                        StatusId = g.st.StatusId,
                        AssembledBy = emp != null ? emp.EmployeeName : "Brak"
                    }
                ).ToListAsync();
            return new ServiceResult<IEnumerable<BikeSubRecordDto>>(ServiceStatus.Ok, string.Empty, bikes);
        }

        public async Task<ServiceResult> DeleteBike(int id)
        {
            var bike = await _context.Bikes.FindAsync(id);
            if (bike == null)
            {
                return new ServiceResult(ServiceStatus.NotFound, "Nie znaleziono roweru");
            }
            _context.Bikes.Remove(bike);
            await _context.SaveChangesAsync();
            return new ServiceResult(ServiceStatus.NoContent, string.Empty);
        }

        public async Task<ServiceResult> PostBike(BikeDto bikeDto)
        {
            if(!bikeDto.PlaceId.HasValue)
            {
                return new ServiceResult(ServiceStatus.BadRequest, "Brak miejsca");
            }
            if (!bikeDto.ModelId.HasValue)
            {
                return new ServiceResult(ServiceStatus.BadRequest, "Brak modelu");
            }
            if (!bikeDto.StatusId.HasValue)
            {
                return new ServiceResult(ServiceStatus.BadRequest, "Brak statusu");
            }
            var bike = new Bike
            {
                PlaceId = bikeDto.PlaceId!.Value,
                ModelId = bikeDto.ModelId!.Value,
                StatusId = bikeDto.StatusId!.Value,
                InsertionDate = DateOnly.FromDateTime(DateTime.Today)
            };
            _context.Add(bike);
            await _context.SaveChangesAsync();
            return new ServiceResult(ServiceStatus.Ok, string.Empty);
        }
    }
}
