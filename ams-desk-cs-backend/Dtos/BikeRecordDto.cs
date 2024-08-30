namespace ams_desk_cs_backend.Dtos
{
    public class BikeRecordDto
    {
        public int ModelId { get; set; }

        public string ProductCode { get; set; } = null!;

        public string EanCode { get; set; } = null!;

        public string ModelName { get; set; } = null!;

        public short FrameSize { get; set; }

        public short WheelSize { get; set; }

        public short ManufacturerId { get; set; }

        public int Price { get; set; }

        public bool IsWoman { get; set; }

        public bool IsElectric { get; set; }

        public int BikeCount { get; set; }

        public IEnumerable<PlaceBikeCountDto> PlaceBikeCount { get; set; }
    }
}
