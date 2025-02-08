namespace ams_desk_cs_backend.BikeApp.Infrastructure.Data.Models.Repairs
{
    public class RepairStatus
    {
        public short RepairStatusId { get; set; }
        public string Name { get; set; } = null!;
        public string Color { get; set; } = null!;
        public virtual ICollection<Repair> Repairs { get; set; } = [];
    }
}
