namespace ams_desk_cs_backend.BikeApp.Infrastructure.Data.Models.Repairs
{
    public class PartUsed
    {
        public int PartUsedId { get; set; }
        public int PartId { get; set; }
        public int RepairId { get; set; }
        public Part? Part { get; set; }
        public Repair? Repair { get; set; }
    }
}
