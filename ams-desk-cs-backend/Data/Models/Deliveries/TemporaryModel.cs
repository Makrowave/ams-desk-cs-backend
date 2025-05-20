namespace ams_desk_cs_backend.Data.Models.Deliveries;

public class TemporaryModel
{
    public int Id { get; set; }
    public int DeliveryId { get; set; }
    public string? ProductCode { get; set; }
    public string? EanCode { get; set; }
    public string? ModelName { get; set; }
    public short? FrameSize { get; set; }
    public bool? IsWoman { get; set; }
    public decimal? WheelSizeId { get; set; }
    public short? ManufacturerId { get; set; }
    public short? ColorId { get; set; }
    public short? CategoryId { get; set; }
    public string? PrimaryColor { get; set; }
    public string? SecondaryColor { get; set; }
    public int? Price { get; set; }
    public bool? IsElectric { get; set; }
    public virtual Manufacturer? Manufacturer { get; set; }
    public virtual Color? Color { get; set; }
    public virtual Category? Category { get; set; }
    public virtual WheelSize? WheelSize { get; set; }
    public Delivery? Delivery { get; set; }
}