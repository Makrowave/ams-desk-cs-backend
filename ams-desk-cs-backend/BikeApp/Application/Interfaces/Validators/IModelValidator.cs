using ams_desk_cs_backend.BikeApp.Dtos.AppModelDto;

namespace ams_desk_cs_backend.BikeApp.Application.Interfaces.Validators
{
    public interface IModelValidator
    {
        public bool ValidateModel(ModelDto model);
        public bool ValidateProductCode(string? code);
        public bool ValidateEanCode(string? code);
        public bool ValidateModelName(string? name);
        public bool ValidateFrameSize(int? size);
        public bool ValidateWheelSize(int? size);
        public bool ValidateColor(string? color);
        public bool ValidatePrice(int? price);
        public bool ValidateLink(string? link);
    }
}
