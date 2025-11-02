using ams_desk_cs_backend.Data.Models.Deliveries;

namespace ams_desk_cs_backend.Deliveries.Dtos;

public record DeliveryItemDto
{
    public DeliveryItemDto() {}

    public DeliveryItemDto(DeliveryItem deliveryItem)
    {
        Id = deliveryItem.Id;
        DeliveryDocumentId = deliveryItem.DeliveryDocumentId;
        ModelId = deliveryItem.ModelId;
        TemporaryModelId = deliveryItem.TemporaryModelId;
        Count = deliveryItem.Count;
        StorageCount = deliveryItem.StorageCount;

        if (deliveryItem.Model is not null)
            DeliveryModel = new DeliveryModelDto(deliveryItem.Model);
        else if (deliveryItem.TemporaryModel is not null)
            DeliveryModel = new DeliveryModelDto(deliveryItem.TemporaryModel);
    }

    public int Id { get; init; }
    public int DeliveryDocumentId { get; init; }
    public int? ModelId { get; init; }
    public int? TemporaryModelId { get; init; }
    public int Count { get; init; }
    public int StorageCount { get; init; }
    public DeliveryModelDto? DeliveryModel { get; init; }
}