namespace ams_desk_cs_backend.BikeApp.Data.Models.Repairs
{
    public class PartType
    {
        public short PartTypeId { get; set; }
        public string PartTypeName { get; set; } = null!;
        public short PartCategoryId { get; set; }
        public ICollection<Part> Parts { get; set; } = [];
        public PartCategory? PartCategory { get; set; }
    }
}
