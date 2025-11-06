using ams_desk_cs_backend.BikeFilters.Enums;
using ams_desk_cs_backend.Bikes.Dtos;
using ams_desk_cs_backend.Bikes.Interfaces;
using ams_desk_cs_backend.Data;
using ams_desk_cs_backend.Data.Models;
using ams_desk_cs_backend.Shared.Results;
using Microsoft.EntityFrameworkCore;

namespace ams_desk_cs_backend.Bikes.Services;

public class BikesService : IBikesService
{
    private readonly BikesDbContext _context;
    public BikesService(BikesDbContext dbContext)
    {
        _context = dbContext;
    }

    public async Task<ServiceResult<BikeSubRecordDto>> PutBike(int id, BikeDto bike)
    {
        var existingBike = await _context.Bikes.FindAsync(id);
        if (existingBike == null)
        {
            return ServiceResult<BikeSubRecordDto>.NotFound("Nie znaleziono roweru");
        }
        if (!(await _context.Places.FindAsync(bike.PlaceId) == null) && bike.PlaceId != null)
        {
            existingBike.PlaceId = bike.PlaceId.Value;
        }
        if (!(await _context.Statuses.FindAsync(bike.StatusId) == null) && bike.StatusId != null)
        {
            existingBike.StatusId = bike.StatusId.Value;
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
        var result = new BikeSubRecordDto
        {
            Id = id,
            Place = existingBike.PlaceId,
            StatusId = existingBike.StatusId,
            AssembledBy = existingBike.AssembledBy,
        };
        return new ServiceResult<BikeSubRecordDto>(ServiceStatus.Ok, string.Empty, result);
    }

    public async Task<ServiceResult<IEnumerable<BikeSubRecordDto>>> GetBikes(int modelId, short placeId)
    {
        if (await _context.Models.FindAsync(modelId) == null)
        {
            return ServiceResult<IEnumerable<BikeSubRecordDto>>.NotFound("Nie znaleziono modelu");
        }
        if (placeId != 0 && await _context.Places.FindAsync(placeId) == null)
        {
            return ServiceResult<IEnumerable<BikeSubRecordDto>>.NotFound("Nie znaleziono miejsca");
        }
        var bikes = await _context.Bikes
            .Where(bi => bi.ModelId == modelId && bi.StatusId != (short)BikeStatus.Sold && (placeId == 0 || bi.PlaceId == placeId))
            .GroupJoin(
                _context.Places,
                bi => bi.PlaceId,
                pl => pl.Id,
                (bi, pl) => new { bi, pl }
            )
            .SelectMany(
                g => g.pl.DefaultIfEmpty(),
                (g1, pl) => new { g1.bi, pl }
            )
            .GroupJoin(
                _context.Statuses,
                g => g.bi.StatusId,
                st => st.Id,
                (g, st) => new { g.bi, g.pl, st }
            )
            .SelectMany(
                g => g.st.DefaultIfEmpty(),
                (g, st) => new { g.bi, g.pl, st }
            )
            .OrderBy(g => g.st.Order).ThenBy(g => g.pl.Id)
            .Select(
                g => new BikeSubRecordDto
                {
                    Id = g.bi.Id,
                    Place = g.bi.PlaceId,
                    StatusId = g.bi.StatusId,
                    AssembledBy = g.bi.AssembledBy
                }
            )

            .ToListAsync();
        return new ServiceResult<IEnumerable<BikeSubRecordDto>>(ServiceStatus.Ok, string.Empty, bikes);
    }

    public async Task<ServiceResult<(short PlaceId, int ModelId)>> SellBike(int id, int price, bool internet)
    {
        var bike = await _context.Bikes.Include(bike => bike.Place).FirstOrDefaultAsync(bike => bike.Id == id);
        if (bike == null)
        {
            return ServiceResult<(short PlaceId, int ModelId)>.NotFound("Nie znaleziono roweru");
        }

        if (bike.StatusId == (short)BikeStatus.Sold)
        {
            return ServiceResult<(short PlaceId, int ModelId)>.BadRequest("Rower już był sprzedany");
        }

        if (price <= 0)
        {
            return ServiceResult<(short PlaceId, int ModelId)>.BadRequest("Cena nie może być <= 0");
        }

        if (bike.Place!.IsStorage)
        {
            return ServiceResult<(short PlaceId, int ModelId)>.BadRequest("Nie można sprzedać roweru z magazynu");
        }
        
        bike.StatusId = (short)BikeStatus.Sold;
        bike.SalePrice = price;
        bike.SaleDate = DateOnly.FromDateTime(DateTime.Now);
        bike.InternetSale = internet;
        await _context.SaveChangesAsync();
        var result = (bike.PlaceId, bike.ModelId);
        return new ServiceResult<(short PlaceId, int ModelId)>(ServiceStatus.Ok, string.Empty, result);
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

    public async Task<ServiceResult<BikeSubRecordDto>> MoveBike(int id, short placeId)
    {
        if (await _context.Places.FindAsync(placeId) == null)
        {
            return ServiceResult<BikeSubRecordDto>.NotFound("Nie znaleziono miejsca docelowego");
        }
        var bike = await _context.Bikes.FindAsync(id);
        if (bike == null)
        {
            return ServiceResult<BikeSubRecordDto>.NotFound("Nie znaleziono roweru");
        }
        bike.PlaceId = placeId;
        await _context.SaveChangesAsync();
        return new ServiceResult<BikeSubRecordDto>(ServiceStatus.Ok, string.Empty, new BikeSubRecordDto
        {
            Id = bike.Id,
            Place = bike.PlaceId,
            StatusId = bike.StatusId,
            AssembledBy = bike.AssembledBy,
        });
    }

    public async Task<ServiceResult<BikeSubRecordDto>> AssembleBikeMobile(int id, short employeeId)
    {
        if (await _context.Employees.FindAsync(employeeId) == null)
        {
            return ServiceResult<BikeSubRecordDto>.NotFound("Nie znaleziono pracownika");
        }
        var bike = await _context.Bikes.FindAsync(id);
        if (bike == null)
        {
            return ServiceResult<BikeSubRecordDto>.NotFound("Nie znaleziono roweru");
        }

        if (bike.StatusId != (short)BikeStatus.Warranty || bike.StatusId != (short)BikeStatus.NotAssembled)
        {
            return ServiceResult<BikeSubRecordDto>.BadRequest("Nie można złożyć złożonego roweru");
        }
        bike.AssembledBy = employeeId;
        bike.StatusId = (short)BikeStatus.Assembled;
        await _context.SaveChangesAsync();
        return new ServiceResult<BikeSubRecordDto>(ServiceStatus.Ok, string.Empty, new BikeSubRecordDto
        {
            Id = bike.Id,
            Place = bike.PlaceId,
            StatusId = bike.StatusId,
            AssembledBy = bike.AssembledBy,
        });
    }

    public async Task<ServiceResult<BikeSubRecordDto>> PostBike(BikeDto bikeDto)
    {
        if (!bikeDto.PlaceId.HasValue)
        {
            return ServiceResult<BikeSubRecordDto>.NotFound("Brak miejsca");
        }
        if (!bikeDto.ModelId.HasValue)
        {
            return ServiceResult<BikeSubRecordDto>.NotFound("Brak modelu");
        }
        if (!bikeDto.StatusId.HasValue)
        {
            return ServiceResult<BikeSubRecordDto>.NotFound( "Brak statusu");
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
        var result = new BikeSubRecordDto
        {
            Id = bike.Id,
            Place = bike.PlaceId,
            StatusId = bike.StatusId,
            AssembledBy = bike.AssembledBy,
        };
        return new ServiceResult<BikeSubRecordDto>(ServiceStatus.Ok, string.Empty, result);
    }
}