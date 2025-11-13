using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ams_desk_cs_backend.Data.Models.Deliveries;

[Table("deliveries")]
public class Delivery
{
    [Key]
    [Column("delivery_id")]
    public int Id { get; set; }
    
    [Column("planned_arrival")]
    public DateTime PlannedArrivalDate { get; set; }
    
    [Column("start_date")]
    public DateTime? StartDate { get; set; }
    
    [Column("finish_date")]
    public DateTime? FinishDate { get; set; }

    [Column("invoice_id")]
    public int? InvoiceId { get; set; }
    
    [Column("placeId")]
    public short PlaceId { get; set; }
    
    [Column("status")]
    public int Status { get; set; }
    
    [ForeignKey(nameof(PlaceId))]
    [InverseProperty(nameof(Place.Deliveries))]
    public virtual Place? Place { get; set; }
    
    [ForeignKey(nameof(InvoiceId))]
    [InverseProperty(nameof(Invoice.Delivery))]
    public virtual Invoice? Invoice { get; set; }
    
    public virtual ICollection<DeliveryDocument> DeliveryDocuments { get; set; } = new List<DeliveryDocument>();
}