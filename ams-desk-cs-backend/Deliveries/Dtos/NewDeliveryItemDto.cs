namespace ams_desk_cs_backend.Deliveries.Dtos;

public record NewDeliveryItemDto
{
    public int DeliveryDocumentId { get; init; }
    public int? ModelId { get; init; }
    public string? Ean { get; init; }
}