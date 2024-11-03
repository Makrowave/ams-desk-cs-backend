namespace ams_desk_cs_backend.BikeApp.Infrastructure.Data.Models
{
    public partial class Color
    {
        public short ColorId { get; set; }
        public required string ColorName { get; set; }
        public required string HexCode { get; set; }
        public ICollection<Model> Models { get; set; } = new List<Model>();
    }
}
