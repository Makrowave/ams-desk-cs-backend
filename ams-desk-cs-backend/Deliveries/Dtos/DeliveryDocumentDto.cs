using System.ComponentModel.DataAnnotations;
using ams_desk_cs_backend.Data.Models.Deliveries;

namespace ams_desk_cs_backend.Deliveries.Dtos;

public record DeliveryDocumentDto
{
    public DeliveryDocumentDto() {}

    public DeliveryDocumentDto(DeliveryDocument deliveryDocument)
    {
        Id = deliveryDocument.Id;
        Name = deliveryDocument.Name;
        Date = deliveryDocument.DocumentDate;
        DeliveryId = deliveryDocument.DeliveryId;
        Items = deliveryDocument.DeliveryItems.Select(deliveryItem => new DeliveryItemDto(deliveryItem)).ToList();
    }
    
    
    public int Id { get; init; }
    public DateTime Date { get; init; }
    [MaxLength(60)] public string Name { get; init; } = null!;
    public int DeliveryId { get; init; }
    
    public ICollection<DeliveryItemDto>? Items { get; init; }
}