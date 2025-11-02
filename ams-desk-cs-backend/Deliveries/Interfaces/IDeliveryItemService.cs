using ams_desk_cs_backend.Deliveries.Dtos;

namespace ams_desk_cs_backend.Deliveries.Interfaces;

public interface IDeliveryItemService
{
    public abstract Task<DeliveryItemDto> AddDeliveryItemAsync(DeliveryItemDto deliveryItemDto);
    public abstract Task DeleteDeliverItemAsync(DeliveryItemDto deliveryItemDto);
    public abstract Task<DeliveryItemDto> IncrementCountAsync(DeliveryItemDto deliveryItemDto);
    public abstract Task<DeliveryItemDto> DecrementCountAsync(DeliveryItemDto deliveryItemDto);
}