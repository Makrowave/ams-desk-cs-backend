namespace ams_desk_cs_backend.Data.Models.Deliveries;

public class Delivery
{
    public int Id { get; set; }
    public DateTime Date { get; set; }
    public string InvoiceNumber { get; set; } = null!;
    public string DeliveryNumber { get; set; } = null!;
    public List<TemporaryModel> TemporaryModels { get; set; } = [];
    public List<Bike> Bikes { get; set; } = [];
}