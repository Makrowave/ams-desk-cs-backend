using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ams_desk_cs_backend.Data.Models.Deliveries;

[Table("delivery_item")]
public class DeliveryItem
{
    [Key]
    [Column("delivery_item_id")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    
    [Column("delivery_document_id")]
    public int DeliveryDocumentId { get; set; }
    
    [Column("model_id")]
    public int? ModelId { get; set; }
    
    [Column("temporary_model_id")]
    public int? TemporaryModelId { get; set; }
    
    [Column("purchase_cost", TypeName = "decimal(10,2)")]
    public decimal PurchaseCost { get; set; }
    
    [Column("item_count")]
    public int Count { get; set; }
    
    [Column("storage_count")]
    public int StorageCount { get; set; }
    
    
    [ForeignKey(nameof(DeliveryDocumentId))]
    [InverseProperty(nameof(DeliveryDocument.DeliveryItems))]
    public DeliveryDocument? DeliveryDocument { get; set; }
    
    
    [ForeignKey(nameof(ModelId))]
    [InverseProperty(nameof(Model.DeliveryItems))]
    public Model? Model { get; set; }
    
    
    [ForeignKey(nameof(TemporaryModelId))]
    [InverseProperty(nameof(TemporaryModel.DeliveryItems))]
    public TemporaryModel? TemporaryModel { get; set; }
    
    
}