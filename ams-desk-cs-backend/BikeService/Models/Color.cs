namespace ams_desk_cs_backend.BikeService.Models
{
    public partial class Color
    {
        public short ColorId { get; set; }
        public required string ColorName { get; set; }
        public ICollection<Model> Models { get; set; } = new List<Model>();
    }
}
