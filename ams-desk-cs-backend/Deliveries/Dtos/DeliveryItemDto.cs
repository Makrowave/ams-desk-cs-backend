using ams_desk_cs_backend.Data.Models.Deliveries;

namespace ams_desk_cs_backend.Deliveries.Dtos;

public record DeliveryItemDto
{
    public DeliveryItemDto() {}

    public DeliveryItemDto(DeliveryItem deliveryItem)
    {
        Id = deliveryItem.Id;
        DeliveryId = deliveryItem.DeliveryId;
        ModelId = deliveryItem.ModelId;
        TemporaryModelId = deliveryItem.TemporaryModelId;
        Count = deliveryItem.Count;

        if (deliveryItem.Model is not null)
            DeliveryModel = new DeliveryModelDto(deliveryItem.Model);
        else if (deliveryItem.TemporaryModel is not null)
            DeliveryModel = new DeliveryModelDto(deliveryItem.TemporaryModel);
    }

    public int Id { get; init; }
    public int DeliveryId { get; init; }
    public int? ModelId { get; init; }
    public int? TemporaryModelId { get; init; }
    public int Count { get; init; }
    public DeliveryModelDto? DeliveryModel { get; init; }
}