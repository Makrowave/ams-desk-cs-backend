using ams_desk_cs_backend.Data;
using ams_desk_cs_backend.Data.Models.Repairs;
using ams_desk_cs_backend.Statistics.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ams_desk_cs_backend.Statistics.Controllers;

[Authorize(Policy = "AccessToken")]
[Route("api/[controller]")]
[ApiController]
public class RepairsDataController: ControllerBase
{
    private readonly BikesDbContext _context;
    
    public RepairsDataController(BikesDbContext context)
    {
        _context = context;
    }
    
    [HttpGet("repairsSum")]
    public async Task<IActionResult> GetSalesInPeriod(
        [FromQuery] DateTime? since,
        [FromQuery] DateTime? until,
        [FromQuery] string interval)
    {
        // Validate interval
        if (interval != "year" && interval != "month" && interval != "day")
        {
            return BadRequest("Invalid interval - should be 'year', 'month', 'day'");
        }
        // Create ranges
        var dateRange = await PrepareDatesAsync(since, until);
        if (dateRange.Since == null || dateRange.Until == null)
        {
            return Ok(new List<Object>());
        }

        var places = await _context.Places.Where(place => !place.IsStorage).ToListAsync();
        var result = new List<SeriesDto<DateAndPriceDto>>();

        //Create series
        foreach (var place in places)
        {
            result.Add(new SeriesDto<DateAndPriceDto>
                {
                    Label = place.PlaceName,
                    Data = (await CreateSeriesAsync(dateRange.Since.Value, dateRange.Until.Value, interval,
                        place.PlaceId)).ToList(),
                }
            );
        }

        var dates = StatisticsHelper.CreateDates(dateRange.Since.Value, dateRange.Until.Value, interval);
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
    
    /// <summary>
    /// Creates Enumerable of sum of bikes sold in date range with some summing interval.
    /// </summary>
    /// <param name="since">Date range start</param>
    /// <param name="until">Date range end</param>
    /// <param name="interval">Grouping interval - day, month, year. Defaults to day</param>
    /// <param name="placeId">Id of place to create the series. -1 for internet sales</param>
    /// <returns>Enumerable of DateAndPriceDto - Date and Int</returns>
    public async Task<IEnumerable<DateAndPriceDto>> CreateSeriesAsync(DateOnly since, DateOnly until, string interval,
        short placeId)
    {
        // Create tuples of dates and prices
        IEnumerable<(DateOnly Date, int Price)> dates =
            StatisticsHelper.CreateDates(since, until, interval).Select(date =>
                (
                    Date: date,
                    Price: 0
                )
            );
        IQueryable<Repair> repairsQuery;
        repairsQuery = _context.Repairs.Where(repair => repair.PlaceId == placeId)
            .Include(repair => repair.Place)
            .Include(repair => repair.Parts)
            .Include(repair => repair.Services)
            .Where(repair => repair.StatusId == (short)RepairStatuses.Collected);
        
        // Bikes filtered by date
        var repairs = await FilterByDateQueryable(repairsQuery, since, until).ToListAsync();
        //Group by interval
        var groupResult = interval switch
        {
            "year" => repairs.GroupBy(repair => repair.CollectionDate!.Value.Year)
                .Select(repairGroup => new
                {
                    Date = new DateOnly(repairGroup.Key, 1, 1),
                    Price = repairGroup.Sum(repair => repair.GetTotalPrice())
                }),
            "month" => repairs.GroupBy(sale => new { sale.CollectionDate!.Value.Year, sale.CollectionDate!.Value.Month })
                .Select(bikeGroup => new
                {
                    Date = new DateOnly(bikeGroup.Key.Year, bikeGroup.Key.Month, 1),
                    Price = bikeGroup.Sum(repair => repair.GetTotalPrice())
                }),
            _ => repairs.GroupBy(sale => sale.CollectionDate!.Value)
                .Select(bikeGroup => new
                {
                    Date = bikeGroup.Key,
                    Price = bikeGroup.Sum(bike => bike.GetTotalPrice())
                })
        };
        // Order by date
        var queryResult = groupResult.OrderBy(bike => bike.Date)
            .ToDictionary(record => record.Date, record => record.Price);
        // Return sums for each interval
        var result = dates
            .Select(record => new DateAndPriceDto
            {
                Date = record.Date,
                Price = queryResult.TryGetValue(record.Date, out var sum) ? sum : 0
            }).ToList();
        return result;
    }
    
    private static IQueryable<Repair> FilterByDateQueryable(IQueryable<Repair> repairs, DateOnly since, DateOnly until)
    {
        return repairs.Where(repair => repair.CollectionDate != null
                                       && repair.CollectionDate.Value >= since
                                       && repair.CollectionDate.Value <= until
        );
    }
    
    private async Task<(DateOnly? Since, DateOnly? Until)> PrepareDatesAsync(DateTime? since, DateTime? until)
    {
        return await StatisticsHelper.PrepareDatesAsync(since, until, _context);
    }
}