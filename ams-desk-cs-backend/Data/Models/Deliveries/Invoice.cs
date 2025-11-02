using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ams_desk_cs_backend.Data.Models.Deliveries;

[Table("invoice")]
public class Invoice
{
    [Key]
    [Column("invoice_id")]
    public int Id { get; set; }
    
    [MaxLength(60)]
    [Column("invoice_number")]
    public required string InvoiceNumber { get; set; }
    
    [Column("issue_date")]
    public DateOnly IssueDate { get; set; }
    
    [Column("payment_date")]
    public DateOnly PaymentDate { get; set; }
    
    [Column("issuer_name")]
    [MaxLength(80)]
    public required string IssuerName { get; set; }
    
    [Column("issuer_address")]
    [MaxLength(100)]
    public required string IssuerAddress { get; set; }
    
    [Column("netto_amount")]
    public decimal NettoAmount { get; set; }
    
    [Column("brutto_amount")]
    public decimal BruttoAmount { get; set; }
    
    [Column("delivery_id")]
    public int DeliveryId { get; set; }
    
    [ForeignKey(nameof(DeliveryId))]
    [InverseProperty(nameof(Delivery.Invoice))]
    public Delivery? Delivery { get; set; }
}