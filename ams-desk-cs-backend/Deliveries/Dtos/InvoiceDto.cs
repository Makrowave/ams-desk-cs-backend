using System.ComponentModel.DataAnnotations;
using ams_desk_cs_backend.Data.Models.Deliveries;

namespace ams_desk_cs_backend.Deliveries.Dtos;

public record InvoiceDto
{
    public InvoiceDto() {}

    public InvoiceDto(Invoice invoice)
    {
        Id = invoice.Id;
        InvoiceNumber = invoice.InvoiceNumber;
        IssueDate = invoice.IssueDate;
        PaymentDate = invoice.PaymentDate;
        IssuerName = invoice.IssuerName;
        IssuerAddress = invoice.IssuerAddress;
        NettoAmount = invoice.NettoAmount;
        BruttoAmount = invoice.BruttoAmount;
        DeliveryId = invoice.DeliveryId;
    }

    public int Id { get; init; }
    
    [MaxLength(60)]
    public string InvoiceNumber { get; init; } = string.Empty;
    
    public DateOnly IssueDate { get; init; }
    
    public DateOnly PaymentDate { get; init; }
    
    public string IssuerName { get; init; } = string.Empty;
    
    public string IssuerAddress { get; init; } = string.Empty;
    
    public decimal NettoAmount { get; init; }
    
    public decimal BruttoAmount { get; init; }
    
    public int? DeliveryId { get; init; }
}