using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ams_desk_cs_backend.Data.Models.Repairs;

[Table("part_categories")]
public class PartCategory
{
    [Key]
    [Column("part_category_id")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public short Id { get; set; }

    [Column("part_category_name")]
    [MaxLength(30)]
    public required string Name { get; set; }

    public virtual ICollection<PartType> PartTypes { get; set; } = new List<PartType>();
}