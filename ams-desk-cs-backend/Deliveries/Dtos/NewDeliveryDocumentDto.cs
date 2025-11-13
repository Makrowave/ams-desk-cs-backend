using System.ComponentModel.DataAnnotations;

namespace ams_desk_cs_backend.Deliveries.Dtos;

public record NewDeliveryDocumentDto
{
    public int DeliveryId { get; init; }
    [MaxLength(60)] public string Name { get; init; } = null!;
    
}