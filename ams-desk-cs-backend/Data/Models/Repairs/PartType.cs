using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ams_desk_cs_backend.Data.Models.Repairs;

[Index(nameof(PartTypeName), IsUnique = true)]
[Table("part_types")]
public class PartType
{
    [Key]
    [Column("part_type_id")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public short PartTypeId { get; set; }

    [Column("part_type_name")]
    [MaxLength(50)]
    public required string PartTypeName { get; set; }

    [Column("part_category_id")]
    public short PartCategoryId { get; set; }

    public virtual ICollection<Part> Parts { get; set; } = new List<Part>();

    [ForeignKey(nameof(PartCategoryId))]
    [InverseProperty(nameof(PartCategory.PartTypes))]
    public virtual PartCategory? PartCategory { get; set; }
}