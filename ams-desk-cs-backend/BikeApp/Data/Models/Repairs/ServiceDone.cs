namespace ams_desk_cs_backend.BikeApp.Data.Models.Repairs
{
    public class ServiceDone
    {
        public int ServiceDoneId { get; set; }
        public short ServiceId { get; set; }
        public int RepairId { get; set; }
        public virtual Service? Service { get; set; }
        public virtual Repair? Repair { get; set; }
    }
}
