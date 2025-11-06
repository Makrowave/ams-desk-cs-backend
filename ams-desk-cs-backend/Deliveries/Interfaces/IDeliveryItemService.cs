using ams_desk_cs_backend.Deliveries.Dtos;

namespace ams_desk_cs_backend.Deliveries.Interfaces;

public interface IDeliveryItemService
{
    public Task<DeliveryItemDto> AddDeliveryItemAsync(DeliveryItemDto deliveryItemDto);
    public Task DeleteDeliverItemAsync(DeliveryItemDto deliveryItemDto);
    public Task<DeliveryItemDto> IncrementCountAsync(DeliveryItemDto deliveryItemDto);
    public Task<DeliveryItemDto> DecrementCountAsync(DeliveryItemDto deliveryItemDto);
}