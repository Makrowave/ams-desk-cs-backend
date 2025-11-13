namespace ams_desk_cs_backend.Deliveries.Dtos;

public record UpdateDeliveryDto
{
    public int Id { get; init; }
    public DateTime PlannedArrivalDate { get; init; }
    public DateTime? StartDate { get; init; }
    public DateTime? FinishDate { get; init; }
    
}