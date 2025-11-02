using ams_desk_cs_backend.Data.Models.Deliveries;

namespace ams_desk_cs_backend.Deliveries.Dtos;

public record DeliverySummaryDto
{
    public DeliverySummaryDto(Delivery delivery)
    {
        if (delivery.Place == null)
        {
            throw new NullReferenceException("Delivery place cannot be null");
        }
        Id = delivery.Id;
        Date = delivery.Date;
        InvoiceNumber = delivery.InvoiceNumber;
        DeliveryDocument = delivery.DeliveryDocument;
        PlaceId = delivery.PlaceId;
        Place = delivery.Place.Name;
    }
    
    public int Id { get; init; }
    public DateTime Date { get; init; }
    public string? InvoiceNumber { get; set; }
    public string? DeliveryDocument { get; set; }
    public int PlaceId { get; set; }
    public string Place { get; init; } = null!;
}