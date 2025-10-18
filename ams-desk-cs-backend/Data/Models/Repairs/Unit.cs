using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ams_desk_cs_backend.Data.Models.Repairs;

[Table("units")]
public class Unit
{
    [Key]
    [Column("unit_id")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public short Id { get; set; }

    [Column("unit_name")]
    public required string Name { get; set; }

    [Column("is_discrete")]
    public bool IsDiscrete { get; set; }

    public virtual ICollection<Part> Parts { get; set; } = new List<Part>();
}