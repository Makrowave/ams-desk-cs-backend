using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ams_desk_cs_backend.Data.Models.Repairs;

namespace ams_desk_cs_backend.Data.Models;

[Table("places")]
public partial class Place
{
    [Key]
    [Column("place_id")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public short Id { get; set; }

    [Column("place_name")]
    [MaxLength(16)]
    public required string Name { get; set; }

    [Column("places_order")]
    public required short Order { get; set; }

    [Column("is_storage")]
    public required bool IsStorage { get; set; }

    public virtual ICollection<Bike> Bikes { get; set; } = new List<Bike>();

    public virtual ICollection<Repair> Repairs { get; set; } = new List<Repair>();
}