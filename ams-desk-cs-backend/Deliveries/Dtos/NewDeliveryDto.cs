using System.ComponentModel.DataAnnotations;

namespace ams_desk_cs_backend.Deliveries.Dtos;

public record NewDeliveryDto
{
    public int PlaceId { get; init; }
    
    public int InvoiceId { get; init; }
    
    public DateTime PlannedArrivalDate { get; init; } 
}