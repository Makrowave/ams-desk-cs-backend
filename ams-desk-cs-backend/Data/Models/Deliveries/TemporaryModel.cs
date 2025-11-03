using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ams_desk_cs_backend.Data.Models.Deliveries;

[Table("temporary_models")]
[Index(nameof(EanCode), IsUnique = true)]
public partial class TemporaryModel
{
    [Key]
    [Column("temporary_model_id")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Column("product_code")]
    [MaxLength(30)]
    public string? ProductCode { get; set; }

    [Column("ean_code")]
    [MaxLength(13)]
    public string? EanCode { get; set; }

    [Column("model_name")]
    [MaxLength(50)]
    public string? Name { get; set; }

    [Column("frame_size")]
    public short? FrameSize { get; set; }

    [Column("is_woman")]
    public bool? IsWoman { get; set; }

    [Column("wheel_size", TypeName = "decimal(3,1)")]
    public decimal? WheelSizeId { get; set; }

    [Column("manufacturer_id")]
    public short? ManufacturerId { get; set; }

    [Column("color_id")]
    public short? ColorId { get; set; }

    [Column("category_id")]
    public short? CategoryId { get; set; }

    [Column("primary_color", TypeName = "CHAR(7)")]
    public string? PrimaryColor { get; set; }

    [Column("secondary_color", TypeName = "CHAR(7)")]
    public string? SecondaryColor { get; set; }

    [Column("price")]
    public int? Price { get; set; }

    [Column("is_electric")]
    public bool? IsElectric { get; set; }

    [Column("link")]
    [MaxLength(160)]
    public string? Link { get; set; }

    [Column("insertion_date")]
    public DateOnly InsertionDate { get; set; }

    [ForeignKey(nameof(ManufacturerId))]
    [InverseProperty(nameof(Manufacturer.TemporaryModels))]
    public virtual Manufacturer? Manufacturer { get; set; }

    [ForeignKey(nameof(ColorId))]
    [InverseProperty(nameof(Color.TemporaryModels))]
    public virtual ModelColor? Color { get; set; }

    [ForeignKey(nameof(CategoryId))]
    [InverseProperty(nameof(Category.TemporaryModels))]
    public virtual Category? Category { get; set; }

    [ForeignKey(nameof(WheelSizeId))]
    [InverseProperty(nameof(WheelSize.TemporaryModels))]
    public virtual WheelSize? WheelSize { get; set; }
    
    public virtual ICollection<DeliveryItem> DeliveryItems { get; set; } = new List<DeliveryItem>();
}
