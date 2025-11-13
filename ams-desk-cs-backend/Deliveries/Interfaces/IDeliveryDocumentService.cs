using ams_desk_cs_backend.Data.Models.Deliveries;
using ams_desk_cs_backend.Deliveries.Dtos;
using ErrorOr;

namespace ams_desk_cs_backend.Deliveries.Interfaces;

public interface IDeliveryDocumentService
{
    Task<ErrorOr<List<DeliveryDocumentDto>>> GetDeliveryDocumentsByDeliveryIdAsync(int deliveryId);
    Task<ErrorOr<DeliveryDocumentDto>> UpdateDeliveryDocumentAsync(DeliveryDocumentDto deliveryDocumentDto);
    Task<ErrorOr<DeliveryDocumentDto>> CreateDeliveryDocumentAsync(NewDeliveryDocumentDto deliveryDocumentDto);
    Task<ErrorOr<Success>> DeleteDeliveryDocumentAsync(int id);
}