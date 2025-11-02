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
        
    }
    
    
    public int Id { get; init; }
    public DateTime Date { get; init; }
    [MaxLength(60)] public string Name { get; init; } = null!;
    
    public ICollection<DeliveryItemDto>? Items { get; init; }
}