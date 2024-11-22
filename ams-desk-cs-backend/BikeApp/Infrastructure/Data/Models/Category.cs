namespace ams_desk_cs_backend.BikeApp.Infrastructure.Data.Models
{
    public partial class Category
    {
        public short CategoryId { get; set; }
        public required string CategoryName { get; set; }
        public required short CategoriesOrder { get; set; }
        public ICollection<Model> Models { get; set; } = new List<Model>();
    }
}
