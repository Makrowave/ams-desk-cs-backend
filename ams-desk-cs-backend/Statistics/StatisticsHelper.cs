using ams_desk_cs_backend.Data;
using ams_desk_cs_backend.Data.Models;
using ams_desk_cs_backend.Statistics.Dtos;
using Microsoft.EntityFrameworkCore;

namespace ams_desk_cs_backend.Statistics;

public static class StatisticsHelper
{
    
    /// <summary>
    /// Creates Enumerable containing dates in ascending order. If interval is other than day, returns dates with interval, first day of interval.
    /// Example - since: 01/01/2025, until: 09/03/2025, interval: "month", result: [01/01/2025, 01/02/2025, 01/03/2025]
    /// </summary>
    /// <param name="since">Date range start</param>
    /// <param name="until">Date range end</param>
    /// <param name="interval"></param>
    /// <returns>IEnumerable of DateOnly of dates every interval</returns>
    public static IEnumerable<DateOnly> CreateDates(DateOnly since, DateOnly until, string interval)
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
    public static async Task<IEnumerable<DateAndPriceDto>> CreateSeriesAsync(DateOnly since, DateOnly until, string interval,
        short placeId, BikesDbContext context)
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
        sales = placeId == -1 ? context.Bikes.Where(bike => bike.InternetSale) 
            : context.Bikes.Where(bike => bike.PlaceId == placeId).Where(bike => !bike.InternetSale);
        
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
    public static IQueryable<Bike> FilterByDateQueryable(IQueryable<Bike> bikes, DateOnly since, DateOnly until)
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
    public static async Task<(DateOnly? Since, DateOnly? Until)> PrepareDatesAsync(DateTime? since, DateTime? until, BikesDbContext context)
    {
        DateOnly? sinceOnly = null;
        DateOnly? untilOnly = null;
        if (since != null) sinceOnly = DateOnly.FromDateTime(since.Value);
        if (until != null) untilOnly = DateOnly.FromDateTime(until.Value);
        if (sinceOnly == null || untilOnly == null)
        {
            sinceOnly = await context.Bikes.Where(b => b.SaleDate != null)
                .MinAsync(b => (DateOnly?)b.SaleDate!.Value);

            untilOnly = await context.Bikes.Where(b => b.SaleDate != null)
                .MaxAsync(b => (DateOnly?)b.SaleDate!.Value);
        }

        return (sinceOnly, untilOnly);
    }
}