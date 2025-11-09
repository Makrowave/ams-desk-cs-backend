using System.ComponentModel.DataAnnotations;

namespace ams_desk_cs_backend.Deliveries.Dtos;

public record NewInvoiceDto
{
    [MaxLength(60)]
    public string InvoiceNumber { get; init; } = string.Empty;
    
    public DateOnly IssueDate { get; init; }
    
    public DateOnly PaymentDate { get; init; }
    
    public string IssuerName { get; init; } = string.Empty;
    
    public string IssuerAddress { get; init; } = string.Empty;
    
    public decimal NettoAmount { get; init; }
    
    public decimal BruttoAmount { get; init; }
}