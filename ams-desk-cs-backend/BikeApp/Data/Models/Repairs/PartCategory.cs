namespace ams_desk_cs_backend.BikeApp.Data.Models.Repairs
{
    public class PartCategory
    {
        public short PartCategoryId { get; set; }
        public string PartCategoryName { get; set; } = null!;
        public ICollection<Part> Parts { get; set; } = [];
    }
}
