using ams_desk_cs_backend.Data.Models.Deliveries;
using ams_desk_cs_backend.Deliveries.Dtos;
using ErrorOr;

namespace ams_desk_cs_backend.Deliveries.Interfaces;

public interface IDeliveryItemService
{
    Task<ErrorOr<DeliveryItemDto>> AddDeliveryItemAsync(NewDeliveryItemDto deliveryItemDto);
    Task<ErrorOr<Success>> DeleteDeliveryItemAsync(DeliveryItemDto deliveryItemDto);
    Task<ErrorOr<DeliveryItemDto>> IncrementCountAsync(DeliveryItemDto deliveryItemDto);
    Task<ErrorOr<DeliveryItemDto>> DecrementCountAsync(DeliveryItemDto deliveryItemDto);
    Task<ErrorOr<DeliveryItem>> MoveToStorageAsync(int deliveryItemId);
    Task<ErrorOr<DeliveryDocument>> MoveMultipleToStorageAsync(int deliveryDocumentId);
}