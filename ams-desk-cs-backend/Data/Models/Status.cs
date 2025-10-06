using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ams_desk_cs_backend.Data.Models;

[Index(nameof(StatusName), IsUnique = true)]
[Table("statuses")]
public partial class Status
{
    [Key]
    [Column("status_id")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public short StatusId { get; set; }

    [Column("status_name")]
    [MaxLength(16)]
    public required string StatusName { get; set; }

    [Column("hex_code")]
    [MaxLength(7)]
    public required string HexCode { get; set; }

    [Column("statuses_order")]
    public required short StatusesOrder { get; set; }

    public virtual ICollection<Bike> Bikes { get; set; } = new List<Bike>();
}