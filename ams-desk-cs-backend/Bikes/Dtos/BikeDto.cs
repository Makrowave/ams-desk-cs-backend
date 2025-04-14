namespace ams_desk_cs_backend.Bikes.Dtos;

public class BikeDto
{
    public int? BikeId { get; set; }

    public int? ModelId { get; set; }

    public short? PlaceId { get; set; }

    public short? StatusId { get; set; }

    public DateOnly? InsertionDate { get; set; }

    public DateOnly? SaleDate { get; set; }

    public int? SalePrice { get; set; }
    public short? AssembledBy { get; set; }
}
