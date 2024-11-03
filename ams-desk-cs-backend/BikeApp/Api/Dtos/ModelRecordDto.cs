namespace ams_desk_cs_backend.BikeApp.Api.Dtos
{
    public class ModelRecordDto
    {
        public int ModelId { get; set; }

        public string? ProductCode { get; set; }

        public string? EanCode { get; set; }

        public required string ModelName { get; set; }

        public short FrameSize { get; set; }

        public short WheelSize { get; set; }

        public short ManufacturerId { get; set; }

        public int Price { get; set; }

        public bool IsWoman { get; set; }

        public bool IsElectric { get; set; }

        public int BikeCount { get; set; }
        public string? PrimaryColor { get; set; }
        public string? SecondaryColor { get; set; }
        public short CategoryId { get; set; }
        public short? ColorId { get; set; }
        public string? Link { get; set; }

        public required IEnumerable<PlaceBikeCountDto> PlaceBikeCount { get; set; }
    }
}
