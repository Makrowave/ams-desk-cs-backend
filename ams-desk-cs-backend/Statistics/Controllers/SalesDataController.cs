using System.Reflection.Emit;
using ams_desk_cs_backend.Data;
using ams_desk_cs_backend.Data.Models;
using ams_desk_cs_backend.Statistics.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ams_desk_cs_backend.Statistics.Controllers;


// [Authorize(Policy = "AccessToken")]
[Route("api/[controller]")]
[ApiController]
public class SalesDataController : ControllerBase
{
    private readonly BikesDbContext _context;
    
    public SalesDataController(BikesDbContext context)
    {
        _context = context;
    }

    [HttpGet("sold")]
    public async Task<IActionResult> GetSoldBikes([FromQuery] DateTime? since, [FromQuery] DateTime? until, [FromQuery] int placeId)
    {
        var sinceOnly = DateOnly.MinValue;
        var untilOnly = DateOnly.MaxValue;
        if (since != null) sinceOnly = DateOnly.FromDateTime(since.Value);
        if (until != null) untilOnly = DateOnly.FromDateTime(until.Value);

        var bikes = await _context.Bikes
            .Include(bike => bike.Model)
            .ThenInclude(model => model!.Manufacturer)
            .Include(bike => bike.Place)
            .Where(
                bike => bike.SaleDate != null
                        && bike.SaleDate.Value >= sinceOnly
                        && bike.SaleDate.Value <= untilOnly
                        && (bike.PlaceId == placeId || placeId == 0)
            )
            .OrderByDescending(bike => bike.SaleDate)
            .Select(bike => new SoldBikeDto(bike))
            .ToListAsync();
        return Ok(bikes);
    }
    
    [HttpGet("soldSum")]
    public async Task<IActionResult> GetSalesInPeriod(
        [FromQuery] DateTime? since, 
        [FromQuery] DateTime? until, 
        [FromQuery] string interval)
    {
        if (interval != "year" && interval != "month" && interval != "day")
        {
            return BadRequest("Invalid interval - should be 'year', 'month', 'day'");
        }
        var dateRange = await PrepareDatesAsync(since, until);
        if (dateRange.Since == null || dateRange.Until == null)
        {
            return Ok(new List<Object>());
        }
        
        var places = await _context.Places.ToListAsync();
        var result = new List<SeriesDto<DateAndPriceDto>>();
        
        //Create series
        foreach (var place in places)
        {
            result.Add(new SeriesDto<DateAndPriceDto>
                {
                    Label = place.PlaceName,
                    Data = (await CreateSeriesAsync(dateRange.Since.Value, dateRange.Until.Value, interval, place.PlaceId)).ToList(),
                }
            );
        }
        
        var dates = CreateDates(dateRange.Since.Value, dateRange.Until.Value, interval);
        var finalResult = new List<Dictionary<string, object>>();
        foreach (var date in dates)
        {
            var dict = new Dictionary<string, object>();
            dict.Add("date", date);
            finalResult.Add(dict);
        }

        foreach (var series in result)
        {
            for (var i = 0; i < finalResult.Count; i++)
            {
                finalResult[i][series.Label] = series.Data[i].Price;
            }
        }
        return Ok(finalResult);
    }

    [HttpGet("getCategoryStats")]
    public async Task<IActionResult> GetCategoryStats([FromQuery] DateTime? since, [FromQuery] DateTime? until)
    {
        var dateRange = await PrepareDatesAsync(since, until);
        if (dateRange.Since == null || dateRange.Until == null)
        {
            return Ok(new List<Object>());
        }
        var result = await _context.Categories.Include(category => category.Models)
            .ThenInclude(model => model.Bikes)
            .Select(category => new PieChartDto
            {
                Id = category.CategoryId,
                Name = category.CategoryName,
                Quantity = category.Models.Select(model => model.Bikes.Count(bike => bike.SaleDate != null
                                              && bike.SaleDate.Value >= dateRange.Since.Value
                                              && bike.SaleDate.Value <= dateRange.Until.Value
                                              && bike.SalePrice != null)).Sum(),
                Value = category.Models.Select(model => model.Bikes.Where(bike => bike.SaleDate != null
                                              && bike.SaleDate.Value >= dateRange.Since.Value
                                              && bike.SaleDate.Value <= dateRange.Until.Value
                                              && bike.SalePrice != null).Sum(bike => bike.SalePrice)).Sum() ?? 0
            })
            .ToListAsync();
        return Ok(result);
    }

    [HttpGet("getFrameTypeStats")]
    public async Task<IActionResult> GetSalesInPeriod([FromQuery] DateTime? since, [FromQuery] DateTime? until)
    {
        var dateRange = await PrepareDatesAsync(since, until);
        if (dateRange.Since == null || dateRange.Until == null)
        {
            return Ok(new List<Object>());
        }
        var bikes = _context.Bikes.Where(bike => bike.SaleDate != null);
        bikes = FilterByDateQueryable(bikes, dateRange.Since.Value, dateRange.Until.Value);
        var result = await bikes
            .Include(bike => bike.Model)
            .GroupBy(bike => bike.Model!.IsWoman)
            .Select(group => new PieChartDto
            {
                Id = group.Key ? 2 : 1,
                Name = group.Key ? "Damska" : "Męska",
                Quantity = group.Count(),
                Value = group.Sum(bike => bike.SalePrice!.Value)
            })
            .ToListAsync();
        return Ok(result);
    }

    private IEnumerable<DateOnly> CreateDates(DateOnly since, DateOnly until, string interval)
    {
        if (interval == "year")
        {
            since = new DateOnly(since.Year, 1, 1);
            return Enumerable.Range(0, (until.Year - since.Year) + 1)
                .Select(since.AddYears);
        }
        if (interval == "month")
        {
            since = new DateOnly(since.Year, since.Month, 1);
            return Enumerable.Range(0, (until.Year - since.Year) * 12 + (until.Month - since.Month) + 1)
                .Select(since.AddMonths);
        }
        return Enumerable.Range(0, (until.DayNumber - since.DayNumber) + 1)
            .Select(since.AddDays);
    }


    private async Task<IEnumerable<DateAndPriceDto>> CreateSeriesAsync(DateOnly since, DateOnly until, string interval, short placeId)
    {
        IEnumerable<(DateOnly Date, int Price)> dates =
            CreateDates(since, until, interval).Select(date =>
                (
                    Date: date,
                    Price: 0
                )
            );
        var sales = _context.Bikes.Where(bike => bike.PlaceId == placeId);
        sales = FilterByDateQueryable(sales, since, until);
        var groupResult = interval switch
        {
            "year" => sales.GroupBy(sale => sale.SaleDate!.Value.Year)
                .Select(bikeGroup => new
                {
                    Date = new DateOnly(bikeGroup.Key, 1, 1), 
                    Price = bikeGroup.Sum(bike => bike.SalePrice!.Value)
                }),
            "month" => sales.GroupBy(sale => new { sale.SaleDate!.Value.Year, sale.SaleDate!.Value.Month })
                .Select(bikeGroup => new 
                {
                    Date = new DateOnly(bikeGroup.Key.Year, bikeGroup.Key.Month, 1),
                    Price = bikeGroup.Sum(bike => bike.SalePrice!.Value)
                }),
            _ => sales.GroupBy(sale => sale.SaleDate!.Value)
                .Select(bikeGroup => new 
                {
                    Date = bikeGroup.Key, 
                    Price = bikeGroup.Sum(bike => bike.SalePrice!.Value)
                })
        };

        var queryResult = await groupResult.OrderBy(bike => bike.Date)
            .ToDictionaryAsync(record => record.Date, record => record.Price);
        var result = dates
            .Select(record => new DateAndPriceDto
            {
                Date = record.Date,
                Price = queryResult.TryGetValue(record.Date, out var sum) ? sum : 0
            }).ToList();
        return result;
    }

    private IQueryable<Bike> FilterByDateQueryable(IQueryable<Bike> bikes, DateOnly since, DateOnly until)
    {
        return bikes.Where(bike => bike.SaleDate != null
                                   && bike.SaleDate.Value >= since
                                   && bike.SaleDate.Value <= until
                                   && bike.SalePrice != null
        );
    }

    private async  Task<(DateOnly? Since, DateOnly? Until)> PrepareDatesAsync(DateTime? since, DateTime? until)
    {
        DateOnly? sinceOnly = null;
        DateOnly? untilOnly = null;
        if (since != null) sinceOnly = DateOnly.FromDateTime(since.Value);
        if (until != null) untilOnly = DateOnly.FromDateTime(until.Value);
        if (sinceOnly == null || untilOnly == null)
        {
            sinceOnly = await _context.Bikes.Where(b => b.SaleDate != null)
                .MinAsync(b => (DateOnly?)b.SaleDate!.Value);

            untilOnly = await _context.Bikes.Where(b => b.SaleDate != null)
                .MaxAsync(b => (DateOnly?)b.SaleDate!.Value);

        }
        return (sinceOnly, untilOnly);
    }
}