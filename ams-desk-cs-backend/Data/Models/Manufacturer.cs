using ams_desk_cs_backend.Data.Models.Deliveries;

namespace ams_desk_cs_backend.Data.Models;

public partial class Manufacturer
{
    public short ManufacturerId { get; set; }

    public required string ManufacturerName { get; set; }
    public required short ManufacturersOrder { get; set; }

    public virtual ICollection<Model> Models { get; set; } = new List<Model>();
    public virtual ICollection<TemporaryModel> TemporaryModels { get; set; } = new List<TemporaryModel>();
}
