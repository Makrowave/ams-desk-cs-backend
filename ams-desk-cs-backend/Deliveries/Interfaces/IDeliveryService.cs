using ams_desk_cs_backend.Data.Models.Deliveries;
using ams_desk_cs_backend.Deliveries.Dtos;
using ErrorOr;

namespace ams_desk_cs_backend.Deliveries.Interfaces;

public interface IDeliveryService
{
    Task<ErrorOr<List<DeliverySummaryDto>>> GetDeliveries();
    Task<ErrorOr<DeliveryDto>> GetDelivery(int deliveryId);
    Task<ErrorOr<DeliveryDto>> UpdateDelivery(int id, DeliveryDto deliveryDto);
    Task<ErrorOr<DeliveryDto>> AddDelivery(NewDeliveryDto deliveryDto);
    Task<ErrorOr<DeliveryDto>> StartDelivery(int id);
    Task<ErrorOr<DeliveryDto>> FinishDelivery(int id);
    Task<ErrorOr<DeliveryDto>> CancelDelivery(int id);
    Task<ErrorOr<Delivery>> ResolveTemporaryModels(Delivery delivery);
}