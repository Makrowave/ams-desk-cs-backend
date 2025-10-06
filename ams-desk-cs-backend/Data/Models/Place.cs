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
    public short PlaceId { get; set; }

    [Column("place_name")]
    [MaxLength(16)]
    public required string PlaceName { get; set; }

    [Column("places_order")]
    public required short PlacesOrder { get; set; }

    [Column("is_storage")]
    public required bool IsStorage { get; set; }

    public virtual ICollection<Bike> Bikes { get; set; } = new List<Bike>();

    public virtual ICollection<Repair> Repairs { get; set; } = new List<Repair>();
}