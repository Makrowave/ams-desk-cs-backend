namespace ams_desk_cs_backend.BikeApp.Data.Models.Repairs
{
    public class Part
    {
        public int PartId { get; set; }
        public string PartName { get; set; } = null!;
        public float Price { get; set; }
        public short PartCategoryId { get; set; }
        public virtual ICollection<PartUsed> PartsUsed { get; set; } = [];
        public virtual PartCategory? PartCategory { get; set; }
    }
}
