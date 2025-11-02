using ams_desk_cs_backend.Data.Models.Deliveries;

namespace ams_desk_cs_backend.Deliveries.Dtos;

public record DeliverySummaryDto
{
    public DeliverySummaryDto(Delivery delivery)
    {
        if (delivery.Place == null)
        {
            throw new NullReferenceException("Delivery place cannot be null");
        }

        if (delivery.Invoice == null)
        {
            throw new NullReferenceException("Invoice cannot be null");
        }
        
        Id = delivery.Id;
        PlannedArrival = delivery.PlannedArrivalDate;
        StartDate = delivery.StartDate;
        FinishDate = delivery.FinishDate;
        Invoice = delivery.Invoice.InvoiceNumber;
        StatusId = delivery.Status;
        Place = delivery.Place.Name;
    }

    public int Id { get; init; }
    public DateTime PlannedArrival { get; init; }
    public DateTime? StartDate { get; init; }
    public DateTime? FinishDate { get; init; }
    public string Invoice { get; init; }
    public int StatusId { get; init; }
    public string Place { get; init; } = null!;
}