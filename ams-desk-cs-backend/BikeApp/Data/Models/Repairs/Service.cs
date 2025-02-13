namespace ams_desk_cs_backend.BikeApp.Data.Models.Repairs
{
    public class Service
    {
        public short ServiceId { get; set; }
        public string ServiceName { get; set; } = null!;
        public float Price { get; set; }
        public short ServiceCategoryId { get; set; }
        public virtual ServiceCategory? ServiceCategory { get; set; }
        public virtual ICollection<ServiceDone> ServicesDone { get; set; } = [];
    }
}
