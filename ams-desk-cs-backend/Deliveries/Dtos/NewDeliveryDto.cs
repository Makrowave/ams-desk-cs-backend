using System.ComponentModel.DataAnnotations;

namespace ams_desk_cs_backend.Deliveries.Dtos;

public record NewDeliveryDto
{
    public int PlaceId { get; init; }
    
    [MaxLength(60)]
    public required string InvoiceNumber { get; init; }
}