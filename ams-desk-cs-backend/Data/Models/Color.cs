using ams_desk_cs_backend.Data.Models.Deliveries;

namespace ams_desk_cs_backend.Data.Models;

public partial class Color
{
    public short ColorId { get; set; }
    public required string ColorName { get; set; }
    public required string HexCode { get; set; }
    public required short ColorsOrder { get; set; }
    public ICollection<Model> Models { get; set; } = new List<Model>();
    public virtual ICollection<TemporaryModel> TemporaryModels { get; set; } = new List<TemporaryModel>();
}