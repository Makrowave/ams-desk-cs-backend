using System.Reflection.Emit;
using ams_desk_cs_backend.Data;
using ams_desk_cs_backend.Data.Models;
using ams_desk_cs_backend.Statistics.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ams_desk_cs_backend.Statistics.Controllers;

[Authorize(Policy = "AccessToken")]
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
    public async Task<IActionResult> GetSoldBikes([FromQuery] DateTime? since, [FromQuery] DateTime? until,
        [FromQuery] int placeId, [FromQuery] int? page, [FromQuery] int? limit)
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
        if (limit.HasValue)
        {
            bikes = bikes.Take(limit.Value).ToList();
        }

        return Ok(bikes);
    }

    [HttpGet("soldSum")]
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
        places.Add(new Place { PlaceId = -1, PlaceName = "Internet", IsStorage = false, PlacesOrder = 1000});
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
    public async Task<IActionResult> GetFrameTypeStats([FromQuery] DateTime? since, [FromQuery] DateTime? until)
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

    [HttpGet("getElectricShare")]
    public async Task<IActionResult> GetElectricShare([FromQuery] DateTime? since, [FromQuery] DateTime? until)
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
            .GroupBy(bike => bike.Model!.IsElectric)
            .Select(group => new PieChartDto
            {
                Id = group.Key ? 2 : 1,
                Name = group.Key ? "Elektryczny" : "Zwykły",
                Quantity = group.Count(),
                Value = group.Sum(bike => bike.SalePrice!.Value)
            })
            .ToListAsync();
        return Ok(result);
    }

    [HttpGet("getOverallStats")]
    public async Task<IActionResult> GetOverallStats([FromQuery] DateTime? since, [FromQuery] DateTime? until)
    {
        var dateRange = await PrepareDatesAsync(since, until);
        if (dateRange.Since == null || dateRange.Until == null)
        {
            return Ok(new Dictionary<string, object>());
        }

        var bikes = _context.Bikes.Where(bike => bike.SaleDate != null);
        bikes = FilterByDateQueryable(bikes, dateRange.Since.Value, dateRange.Until.Value);
        var stats = await bikes
            .Include(bike => bike.Model)
            .Select(bike => new
            {
                SalePrice = bike.SalePrice!.Value,
                DefaultPrice = bike.Model!.Price,
                DiscountPercent = (bike.Model!.Price - bike.SalePrice!.Value) * 100 / bike.Model!.Price,
            })
            .ToListAsync();

        var result = new Dictionary<string, object>();
        var even = stats.Count % 2 == 0;
        if (stats.Count == 0)
        {
            result["sum"] = 0;
            result["count"] = 0;
            result["median"] = 0;
            result["average"] = 0;
            result["medianDiscount"] = 0;
            return Ok(result);
        }

        result["sum"] = stats.Sum(stat => stat.SalePrice);
        result["count"] = stats.Count;
        result["median"] =
            stats
                .OrderBy(stat => stat.SalePrice)
                .Skip(stats.Count / 2 - (even ? 1 : 0))
                .Take(even ? 2 : 1)
                .Sum(stat => stat.SalePrice) / (even ? 2 : 1);
        result["average"] = stats.Average(stat => stat.SalePrice);
        result["medianDiscount"] =
            stats
                .OrderBy(stat => stat.DiscountPercent)
                .Skip(stats.Count / 2 - (even ? 1 : 0))
                .Take(even ? 2 : 1)
                .Sum(middle => middle.DiscountPercent) / (even ? 2 : 1);


        return Ok(result);
    }

    [HttpGet("getPlacesMedian")]
    public async Task<IActionResult> GetPlacesMedian([FromQuery] DateTime? since, [FromQuery] DateTime? until)
    {
        var dateRange = await PrepareDatesAsync(since, until);
        if (dateRange.Since == null || dateRange.Until == null)
        {
            return Ok(new Dictionary<string, object>());
        }

        var filteredBikes = FilterByDateQueryable(_context.Bikes.Where(bike => bike.SaleDate != null),
            dateRange.Since.Value, dateRange.Until.Value);
        var bikes = (await filteredBikes
            .Include(bike => bike.Place)
            .Where(bike => !bike.Place!.IsStorage)
            .Where(bike => !bike.InternetSale)
            .GroupBy(bike => bike.Place)
            .ToListAsync()).OrderBy(group => group.Key!.PlaceId);

        var result = bikes
            .Select(group =>
            {
                var even = group.Count() % 2 == 0;
                return new
                {
                    Place = group.Key!.PlaceName,
                    Value = group.OrderBy(bike => bike.SalePrice!.Value)
                        .Skip(group.Count() / 2 - (even ? 1 : 0))
                        .Take(even ? 2 : 1)
                        .Sum(middle => middle.SalePrice!.Value) / (even ? 2 : 1)
                };
            }).ToDictionary(group => group.Place, group => (object)group.Value);
        // Internet segment
        var internetBikes = (await filteredBikes.Where(bike => bike.InternetSale).ToListAsync())
            .OrderBy(bike => bike.SalePrice!.Value);
        var internetCount = internetBikes.Count();
        var internetEven = internetCount % 2 == 0;
        var internetMedian = internetBikes.Skip(internetCount / 2 - (internetEven ? 1 : 0))
            .Take(internetEven ? 2 : 1)
            .Sum(middle => middle.SalePrice!.Value) / (internetEven ? 2 : 1);
        result.Add("Internet", internetMedian);
        // Add date
        result.Add("date", $"{dateRange.Since:yyyy-MM-dd} - {dateRange.Until:yyyy-MM-dd}");
        return Ok(new List<Dictionary<string, object>> { result });
    }

    [HttpGet("getPlacesAverage")]
    public async Task<IActionResult> GetPlacesAverage([FromQuery] DateTime? since, [FromQuery] DateTime? until)
    {
        var dateRange = await PrepareDatesAsync(since, until);
        if (dateRange.Since == null || dateRange.Until == null)
        {
            return Ok(new Dictionary<string, object>());
        }

        var filteredBikes = FilterByDateQueryable(_context.Bikes.Where(bike => bike.SaleDate != null),
            dateRange.Since.Value, dateRange.Until.Value);
        var bikes = (await filteredBikes
            .Include(bike => bike.Place)
            .Where(bike => !bike.Place!.IsStorage)
            .Where(bike => !bike.InternetSale)
            .GroupBy(bike => bike.Place)
            .ToListAsync()).OrderBy(group => group.Key!.PlaceId);

        var result = bikes
            .Select(group => new
            {
                Place = group.Key!.PlaceName,
                Value = group.Average(bike => bike.SalePrice!.Value)
            }).ToDictionary(group => group.Place, group => (object)group.Value);
        // Internet segment
        var internetBikes = await filteredBikes.Where(bike => bike.InternetSale).ToListAsync();
        var internetMedian = internetBikes.Average(bike => bike.SalePrice!.Value);
        result.Add("Internet", internetMedian);
        // Add date
        result.Add("date", $"{dateRange.Since:yyyy-MM-dd} - {dateRange.Until:yyyy-MM-dd}");
        return Ok(new List<Dictionary<string, object>> { result });
    }

    [HttpGet("getPlacesMedianDiscount")]
    public async Task<IActionResult> GetPlacesMedianDiscount([FromQuery] DateTime? since, [FromQuery] DateTime? until)
    {
        var dateRange = await PrepareDatesAsync(since, until);
        if (dateRange.Since == null || dateRange.Until == null)
        {
            return Ok(new Dictionary<string, object>());
        }

        var filteredBikes = FilterByDateQueryable(_context.Bikes.Where(bike => bike.SaleDate != null),
            dateRange.Since.Value, dateRange.Until.Value);
        var bikes = (await filteredBikes
            .Include(bike => bike.Place)
            .Include(bike => bike.Model)
            .Where(bike => !bike.Place!.IsStorage)
            .Where(bike => !bike.InternetSale)
            .GroupBy(bike => bike.Place)
            .ToListAsync()).OrderBy(group => group.Key!.PlaceId);

        var result = bikes
            .Select(group =>
            {
                var even = group.Count() % 2 == 0;
                return new
                {
                    Place = group.Key!.PlaceName,
                    Value = group.OrderBy(bike => (bike.Model!.Price - bike.SalePrice!.Value) * 100 / bike.Model!.Price)
                                .Skip(group.Count() / 2 - (even ? 1 : 0))
                                .Take(even ? 2 : 1)
                                .Sum(middle =>
                                    (middle.Model!.Price - middle.SalePrice!.Value) * 100 / middle.Model!.Price) /
                            (even ? 2 : 1)
                };
            }).ToDictionary(group => group.Place, group => (object)group.Value);
        // Internet segment
        var internetBikes = await filteredBikes.Include(bike => bike.Model)
            .Where(bike => bike.InternetSale).ToListAsync();
        var internetCount = internetBikes.Count();
        var internetEven = internetCount % 2 == 0;
        var internetMedian = internetBikes
                                 .OrderBy(bike => (bike.Model!.Price - bike.SalePrice!.Value) * 100 / bike.Model!.Price)
                                 .Skip(internetCount / 2 - (internetEven ? 1 : 0))
                                 .Take(internetEven ? 2 : 1)
                                 .Sum(middle =>
                                     (middle.Model!.Price - middle.SalePrice!.Value) * 100 / middle.Model!.Price) /
                             (internetEven ? 2 : 1);
        result.Add("Internet", internetMedian);
        // Add date
        result.Add("date", $"{dateRange.Since:yyyy-MM-dd} - {dateRange.Until:yyyy-MM-dd}");
        return Ok(new List<Dictionary<string, object>> { result });
    }

    [HttpGet("getPlacesSum")]
    public async Task<IActionResult> GetPlacesSum([FromQuery] DateTime? since, [FromQuery] DateTime? until)
    {
        var dateRange = await PrepareDatesAsync(since, until);
        if (dateRange.Since == null || dateRange.Until == null)
        {
            return Ok(new Dictionary<string, object>());
        }

        var filteredBikes = FilterByDateQueryable(_context.Bikes.Where(bike => bike.SaleDate != null),
            dateRange.Since.Value, dateRange.Until.Value);
        var bikes = (await filteredBikes
            .Include(bike => bike.Place)
            .Where(bike => !bike.Place!.IsStorage)
            .Where(bike => !bike.InternetSale)
            .GroupBy(bike => bike.Place)
            .ToListAsync()).OrderBy(group => group.Key!.PlaceId);

        var result = bikes
            .Select(group => new
            {
                Place = group.Key!.PlaceName,
                Value = group.Sum(bike => bike.SalePrice!.Value)
            }).ToDictionary(group => group.Place, group => (object)group.Value);
        // Internet segment
        var internetBikes = await filteredBikes.Where(bike => bike.InternetSale).ToListAsync();
        var internetMedian = internetBikes.Sum(bike => bike.SalePrice!.Value);
        result.Add("Internet", internetMedian);
        // Add date
        result.Add("date", $"{dateRange.Since:yyyy-MM-dd} - {dateRange.Until:yyyy-MM-dd}");
        return Ok(new List<Dictionary<string, object>> { result });
    }

    [HttpGet("mostPopularCategoryByPlace")]
    public async Task<IActionResult> GetMostPopularCategoryByPlace([FromQuery] DateTime? since,
        [FromQuery] DateTime? until, [FromQuery] bool isCount, [FromQuery] short placeId)
    {
        var place = await _context.Places.FindAsync(placeId);
        if (place == null) return NotFound("Place not found");
        var dateRange = await PrepareDatesAsync(since, until);
        if (dateRange.Since == null || dateRange.Until == null)
        {
            return Ok(new Dictionary<string, object>());
        }
        
        var grouping = await FilterByDateQueryable(_context.Bikes.Where(bike => bike.SaleDate != null),
                dateRange.Since.Value, dateRange.Until.Value)
            .Include(bike => bike.Place)
            .Include(bike => bike.Model)
            .ThenInclude(model => model!.Category)
            .Where(bike => bike.PlaceId == placeId)
            .GroupBy(bike => bike.Model!.Category!.CategoryName)
            .ToListAsync();

        var result = grouping.Select(group => new
            {
                Category = group.Key,
                Value = isCount ? group.Count() : group.Sum(bike => bike.SalePrice!.Value)
            })
            .OrderByDescending(category => category.Value)
            .Take(4)
            .ToList()
            .ToDictionary(group => group.Category, group => (object)group.Value);
        result.Add("place", place.PlaceName);
        return Ok(new List<Dictionary<string, object>> { result });
    }

    /// <summary>
    /// Creates Enumerable containing dates in ascending order. If interval is other than day, returns dates with interval, first day of interval.
    /// Example - since: 01/01/2025, until: 09/03/2025, interval: "month", result: [01/01/2025, 01/02/2025, 01/03/2025]
    /// </summary>
    /// <param name="since">Date range start</param>
    /// <param name="until">Date range end</param>
    /// <param name="interval"></param>
    /// <returns>IEnumerable of DateOnly of dates every interval</returns>
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

    /// <summary>
    /// Creates Enumerable of sum of bikes sold in date range with some summing interval.
    /// </summary>
    /// <param name="since">Date range start</param>
    /// <param name="until">Date range end</param>
    /// <param name="interval">Grouping interval - day, month, year. Defaults to day</param>
    /// <param name="placeId">Id of place to create the series. -1 for internet sales</param>
    /// <returns>Enumerable of DateAndPriceDto - Date and Int</returns>
    private async Task<IEnumerable<DateAndPriceDto>> CreateSeriesAsync(DateOnly since, DateOnly until, string interval,
        short placeId)
    {
        // Create tuples of dates and prices
        IEnumerable<(DateOnly Date, int Price)> dates =
            CreateDates(since, until, interval).Select(date =>
                (
                    Date: date,
                    Price: 0
                )
            );
        IQueryable<Bike> sales;
        // If id is -1 - internet sales, otherwise bikes not sold by internet
        sales = placeId == -1 ? _context.Bikes.Where(bike => bike.InternetSale) 
            : _context.Bikes.Where(bike => bike.PlaceId == placeId).Where(bike => !bike.InternetSale);
        
        // Bikes filtered by date
        sales = FilterByDateQueryable(sales, since, until);
        //Group by interval
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
        // Order by date
        var queryResult = await groupResult.OrderBy(bike => bike.Date)
            .ToDictionaryAsync(record => record.Date, record => record.Price);
        // Return sums for each interval
        var result = dates
            .Select(record => new DateAndPriceDto
            {
                Date = record.Date,
                Price = queryResult.TryGetValue(record.Date, out var sum) ? sum : 0
            }).ToList();
        return result;
    }
    /// <summary>
    /// Finds all sold bikes and filters by date.
    /// </summary>
    /// <param name="bikes">Bikes IQueryable to filter</param>
    /// <param name="since">Date range start</param>
    /// <param name="until">Date range end</param>
    /// <returns>Sold bikes IQueryable filtered by date</returns>
    private IQueryable<Bike> FilterByDateQueryable(IQueryable<Bike> bikes, DateOnly since, DateOnly until)
    {
        return bikes.Where(bike => bike.SaleDate != null
                                   && bike.SaleDate.Value >= since
                                   && bike.SaleDate.Value <= until
                                   && bike.SalePrice != null
        );
    }

    /// <summary>
    /// Creates 2 DateTime dates which are marking date range.
    /// If since and until are null it creates dates based on first and last sale and if no bikes were sold - 2 nulls.
    /// </summary>
    /// <param name="since">First date in range</param>
    /// <param name="until">Last date in range</param>
    /// <returns>Tuple of DateTime dates or 2 nulls if no bikes were ever sold and passed args are nulls</returns>
    private async Task<(DateOnly? Since, DateOnly? Until)> PrepareDatesAsync(DateTime? since, DateTime? until)
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