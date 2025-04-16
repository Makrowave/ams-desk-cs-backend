using ams_desk_cs_backend.Data.Models;

namespace ams_desk_cs_backend.Statistics.Dtos;

public class SoldBikeDto
{
    public SoldBikeDto(Bike bike)
    {
        if (bike.Model?.Manufacturer == null)
        {
            throw new ArgumentException("Invalid model or manufacturer");
        }

        if (bike.Place == null)
        {
            throw new ArgumentException("Invalid place");
        }

        if (bike.SaleDate == null)
        {
            throw new ArgumentException("Invalid date");
        }
        if(bike.SalePrice == null)
        {
            throw new ArgumentException("Invalid price");
        }
        Id = bike.BikeId;
        Model = bike.Model.ModelName;
        Manufacturer = bike.Model.Manufacturer.ManufacturerName;
        PrimaryColor = bike.Model.PrimaryColor;
        SecondaryColor = bike.Model.SecondaryColor;
        Place = bike.Place!.PlaceName;
        Price = bike.Model.Price;
        SalePrice = bike.SalePrice.Value;
        Discount = Price - SalePrice;
        DiscountPercent = (int)((float)Discount / Price * 100.0);
        SaleDate = bike.SaleDate.Value;
    }
    
    
    public int Id { get; set; }
    public string Model { get; set; }
    public string Manufacturer { get; set; }
    public string? PrimaryColor { get; set; }
    public string? SecondaryColor { get; set; }
    public string Place { get; set; }
    public int Price { get; set; }
    public int SalePrice { get; set; }
    public int Discount { get; set; }
    public int DiscountPercent { get; set; }
    public DateOnly SaleDate { get; set; }
}