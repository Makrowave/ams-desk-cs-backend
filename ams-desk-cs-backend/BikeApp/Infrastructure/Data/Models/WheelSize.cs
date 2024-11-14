namespace ams_desk_cs_backend.BikeApp.Infrastructure.Data.Models;

public partial class WheelSize
{
    public required short WheelSizeId { get; set; }
    public virtual ICollection<Model> Models { get; set; } = new List<Model>();
}
