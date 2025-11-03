using System.ComponentModel.DataAnnotations;
using ams_desk_cs_backend.Data.Models;
using ams_desk_cs_backend.Data.Models.Deliveries;
using ams_desk_cs_backend.Deliveries.Enums;

namespace ams_desk_cs_backend.Deliveries.Dtos;

public record DeliveryDto
{
    public DeliveryDto() {}

    public DeliveryDto(Delivery delivery)
    {
        if (delivery.Invoice == null)
        {
            throw new ArgumentException("Invalid delivery invoice.");
        }
        
        Id = delivery.Id;
        PlannedArrivalDate = delivery.PlannedArrivalDate;
        StartDate = delivery.StartDate;
        FinishDate = delivery.FinishDate;
        InvoiceId = delivery.InvoiceId;
        PlaceId = delivery.PlaceId;
        Status = (DeliveryStatus)delivery.Status;
        Place = delivery.Place;
        Invoice = new InvoiceDto(delivery.Invoice);
        DeliveryDocuments = delivery.DeliveryDocuments
            .Select(doc => new DeliveryDocumentDto(doc))
            .ToList();
    }
    
    public int Id { get; init; }
    public DateTime PlannedArrivalDate { get; init; }
    public DateTime? StartDate { get; init; }
    public DateTime? FinishDate { get; init; }
    public int InvoiceId { get; init; }
    public short PlaceId { get; init; }
    public DeliveryStatus Status { get; init; }
    public Place? Place { get; init; }
    public InvoiceDto? Invoice { get; init; }
    public ICollection<DeliveryDocumentDto> DeliveryDocuments { get; init; } = new List<DeliveryDocumentDto>();
}