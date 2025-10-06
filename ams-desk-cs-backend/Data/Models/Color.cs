using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ams_desk_cs_backend.Data.Models;

[Index(nameof(ColorName), IsUnique = true)]
[Table("colors")]
public partial class Color
{
    [Key]
    [Column("color_id")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public short ColorId { get; set; }

    [Column("color_name")]
    [MaxLength(30)]
    public required string ColorName { get; set; }

    [Column("hex_code")]
    [MaxLength(7)]
    public required string HexCode { get; set; }

    [Column("colors_order")]
    public required short ColorsOrder { get; set; }

    public virtual ICollection<Model> Models { get; set; } = new List<Model>();
}