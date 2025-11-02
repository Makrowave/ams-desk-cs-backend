using ams_desk_cs_backend.Deliveries.Dtos;

namespace ams_desk_cs_backend.Deliveries.Interfaces;

public interface ITemporaryModelService
{
    public Task<DeliveryModelDto> UpdateDeliveryModelAsync(DeliveryModelDto deliveryModelDto);
    public Task DeleteDeliveryModelAsync(DeliveryModelDto deliveryModelDto);
}