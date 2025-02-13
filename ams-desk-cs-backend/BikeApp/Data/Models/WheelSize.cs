namespace ams_desk_cs_backend.BikeApp.Data.Models;

public partial class WheelSize
{
    public required short WheelSizeId { get; set; }
    public virtual ICollection<Model> Models { get; set; } = new List<Model>();
}
