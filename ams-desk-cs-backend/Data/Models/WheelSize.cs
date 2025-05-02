namespace ams_desk_cs_backend.Data.Models;

public partial class WheelSize
{
    public required decimal WheelSizeId { get; set; }
    public virtual ICollection<Model> Models { get; set; } = new List<Model>();
}
