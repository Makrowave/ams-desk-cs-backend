using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ams_desk_cs_backend.Data.Models;

[Index(nameof(Name), IsUnique = true)]
[Table("statuses")]
public partial class Status
{
    [Key]
    [Column("status_id")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public short Id { get; set; }

    [Column("status_name")]
    [MaxLength(16)]
    public required string Name { get; set; }

    [Column("hex_code")]
    [MaxLength(7)]
    public required string Color { get; set; }

    [Column("statuses_order")]
    public required short Order { get; set; }

    public virtual ICollection<Bike> Bikes { get; set; } = new List<Bike>();
}