using ams_desk_cs_backend.Data.Models.Deliveries;

namespace ams_desk_cs_backend.Data.Models;

public partial class Category
{
    public short CategoryId { get; set; }
    public required string CategoryName { get; set; }
    public required short CategoriesOrder { get; set; }
    public ICollection<Model> Models { get; set; } = new List<Model>();
    public virtual ICollection<TemporaryModel> TemporaryModels { get; set; } = new List<TemporaryModel>();
}