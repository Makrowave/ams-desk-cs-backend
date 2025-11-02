using ams_desk_cs_backend.Data.Models.Deliveries;
using ams_desk_cs_backend.Deliveries.Dtos;

namespace ams_desk_cs_backend.Deliveries.Interfaces;

public interface IDeliveryService
{
    public abstract Task<List<DeliverySummaryDto>> GetDeliveries();
    public abstract Task<DeliveryDto?> GetDelivery(int deliveryId);
    public abstract Task<DeliveryDto?> UpdateDelivery(DeliveryDto deliveryDto);
    public abstract Task<DeliveryDto> AddDelivery(NewDeliveryDto deliveryDto);
    public abstract Task<DeliveryDto?> StartDelivery(int deliveryId);
    public abstract Task<DeliveryDto?> FinishDelivery(int deliveryId);
    public abstract Task<DeliveryDto?> CancelDelivery(int deliveryId);
    
    public abstract Task ResolveTemporaryModels(int deliveryId);
}