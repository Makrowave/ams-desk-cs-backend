using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ams_desk_cs_backend.Data.Models;

[Table("manufacturers")]
public partial class Manufacturer
{
    [Key]
    [Column("manufacturer_id")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public short ManufacturerId { get; set; }

    [Column("manufacturer_name")]
    [MaxLength(20)]
    public required string ManufacturerName { get; set; }

    [Column("manufacturers_order")]
    public required short ManufacturersOrder { get; set; }

    public virtual ICollection<Model> Models { get; set; } = new List<Model>();
}