using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ams_desk_cs_backend.Data.Models.Deliveries;

namespace ams_desk_cs_backend.Data.Models;

[Table("wheel_sizes")]
public partial class WheelSize
{
    [Key]
    [Column("wheel_size", TypeName = "decimal(3,1)")]
    public required decimal WheelSizeId { get; set; }
    public virtual ICollection<Model> Models { get; set; } = new List<Model>();
    public virtual ICollection<TemporaryModel> TemporaryModels { get; set; } = new List<TemporaryModel>();
}
