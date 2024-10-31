namespace ams_desk_cs_backend.BikeService.Dtos
{
    public class PatchModelDto {
        public required string ProductCode {get; set;}
        public required string ModelName {get; set;}
        public short FrameSize {get; set;}
        public short WheelSize {get; set;}
        public bool IsWoman {get; set;}
        public short ManufacturerId {get; set;}
        public short CategoryId { get; set; }
        public int Price {get; set;}
        public bool IsElectric { get; set; }
    }
}