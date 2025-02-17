namespace ams_desk_cs_backend.BikeApp.Data.Models.Repairs
{
    public class Unit
    {
        public short UnitId { get; set; }
        public string UnitName { get; set; } = null!;
        //Discrete - Integer, Non-discrete - Float
        public bool IsDiscrete { get; set; }
        public virtual ICollection<Part> Parts { get; set; } = [];
    }
}
