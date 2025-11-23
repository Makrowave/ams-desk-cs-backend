using ams_desk_cs_backend.Data.Models.Deliveries;
using ams_desk_cs_backend.Deliveries.Dtos;
using ErrorOr;

namespace ams_desk_cs_backend.Deliveries.Interfaces;

public interface IDeliveryItemService
{
    Task<ErrorOr<DeliveryItemDto>> AddDeliveryItemAsync(NewDeliveryItemDto deliveryItemDto);
    Task<ErrorOr<Success>> DeleteDeliveryItemAsync(int id);
    Task<ErrorOr<int>> IncrementCountAsync(int id);
    Task<ErrorOr<int>> DecrementCountAsync(int id);
    Task<ErrorOr<DeliveryItemDto>> MoveToStorageAsync(int deliveryItemId);
    Task<ErrorOr<Success>> MoveMultipleToStorageAsync(Delivery delivery);
}