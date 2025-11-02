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
        Id = delivery.Id;
        Date = delivery.Date;
        StatusId =  (DeliveryStatus)delivery.StatusId;
        InvoiceNumber = delivery.InvoiceNumber;
        DeliveryDocument = delivery.DeliveryDocument;
        PlaceId = delivery.PlaceId;
        Place = delivery.Place;
        DeliveryItems = delivery.DeliveryItems.Select(item => new DeliveryItemDto(item)).ToList();
    }
    
    public int Id { get; init; }
    
    public DateTime Date { get; init; }
    [MaxLength(60)]
    public string InvoiceNumber { get; init; }
    
    [MaxLength(60)]
    public string? DeliveryDocument { get; init; }
    
    public int PlaceId { get; init; }
    
    public DeliveryStatus StatusId { get; init; }
    
    public Place? Place { get; init; }
    
    public ICollection<DeliveryItemDto> DeliveryItems { get; init; } = new List<DeliveryItemDto>();
    
}