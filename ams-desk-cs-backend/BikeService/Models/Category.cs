namespace ams_desk_cs_backend.BikeService.Models
{
    public partial class Category
    {
        public short CategoryId { get; set; }
        public required string CategoryName { get; set; }
        public ICollection<Model> Models { get; set; } = new List<Model>();
    }
}
