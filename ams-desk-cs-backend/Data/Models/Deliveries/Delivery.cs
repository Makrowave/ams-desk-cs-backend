using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ams_desk_cs_backend.Data.Models.Deliveries;

[Table("deliveries")]
public class Delivery
{
    [Key]
    [Column("delivery_id")]
    public int Id { get; set; }
    
    [Column("date")]
    public DateTime Date { get; set; }

    [Column("invoice_number")]
    [MaxLength(60)]
    public required string InvoiceNumber { get; set; }

    [Column("delivery_document")]
    [MaxLength(60)]
    public string? DeliveryDocument { get; set; }
    
    [Column("placeId")]
    public int PlaceId { get; set; }
    
    [Column("statusId")]
    public int StatusId { get; set; }
    
    
    [ForeignKey(nameof(PlaceId))]
    [InverseProperty(nameof(Place.Deliveries))]
    public virtual Place? Place { get; set; }
    
    public virtual ICollection<DeliveryItem> DeliveryItems { get; set; } = new List<DeliveryItem>();
}