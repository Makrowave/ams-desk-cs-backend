using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ams_desk_cs_backend.Data.Models.Deliveries;

[Table("delivery_document")]
public class DeliveryDocument
{
    [Key]
    [Column("delivery_document_id")]
    public int Id { get; set; }
    
    [MaxLength(60)]
    [Column("delivery_document_name")]
    public required string Name { get; set; }
    
    [Column("delivery_document_id")]
    public int DeliveryId { get; set; }
    
    [Column("delivery_document_date")]
    public DateTime DocumentDate { get; set; }
    
    [ForeignKey(nameof(DeliveryId))]
    [InverseProperty(nameof(Delivery.DeliveryDocuments))]
    public Delivery? Delivery { get; set; }
    
    
    public virtual ICollection<DeliveryItem> DeliveryItems { get; set; } = new List<DeliveryItem>();
}