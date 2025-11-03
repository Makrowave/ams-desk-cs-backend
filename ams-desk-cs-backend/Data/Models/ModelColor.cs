using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ams_desk_cs_backend.Data.Models.Deliveries;
using Microsoft.EntityFrameworkCore;

namespace ams_desk_cs_backend.Data.Models;

[Index(nameof(Name), IsUnique = true)]
[Table("colors")]
public partial class ModelColor
{
    [Key]
    [Column("color_id")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public short Id { get; set; }

    [Column("color_name")]
    [MaxLength(30)]
    public required string Name { get; set; }

    [Column("hex_code")]
    [MaxLength(7)]
    public required string Color { get; set; }

    [Column("colors_order")]
    public required short Order { get; set; }

    public virtual ICollection<Model> Models { get; set; } = new List<Model>();
    public virtual ICollection<TemporaryModel> TemporaryModels { get; set; } = new List<TemporaryModel>();
}