namespace ams_desk_cs_backend.BikeApp.Api.Dtos
{
    public class ModelFilter
    {
        public bool? Avaible { get; set; }
        public bool? Electric { get; set; }
        public int? ManufacturerId { get; set; }
        public int? WheelSize { get; set; }
        public int? FrameSize { get; set; }
        public int? StatusId { get; set; }
        public int? MinPrice { get; set; }
        public int? MaxPrice { get; set; }
        public string? Name { get; set; }
        public bool? IsWoman { get; set; }
        public bool? IsKids { get; set; }
        public int? CategoryId { get; set; }
        public int? ColorId { get; set; }
        public int? PlaceId { get; set; }
        public string? ProductCode { get; set; }
        public bool? NoEan { get; set; }
        public bool? NoProductCode { get; set; }
        public bool? NoColor { get; set; }
        public bool? NoColorGroup { get; set; }
    }
}
