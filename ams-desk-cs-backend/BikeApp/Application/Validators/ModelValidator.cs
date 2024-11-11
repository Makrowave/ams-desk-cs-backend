using ams_desk_cs_backend.BikeApp.Application.Interfaces.Validators;
using ams_desk_cs_backend.BikeApp.Dtos.AppModelDto;
using System.Text.RegularExpressions;

namespace ams_desk_cs_backend.BikeApp.Application.Validators
{
    public class ModelValidator : IModelValidator
    {
        public bool ValidateModel(ModelDto model)
        {
            if (model.IsWoman == null
                || model.ManufacturerId == null
                || model.ColorId == null
                || model.CategoryId == null
                || model.IsElectric == null
                || !ValidateColor(model.PrimaryColor)
                || !ValidateColor(model.SecondaryColor)
                || !ValidateEanCode(model.EanCode)
                || !ValidateModelName(model.ModelName)
                || !ValidateFrameSize(model.FrameSize)
                || !ValidateWheelSize(model.WheelSize)
                || !ValidatePrice(model.Price)
                || !ValidateProductCode(model.ProductCode))
            {
                return false;
            }
            return true;
        }
        //Most likely redundant
        public bool ValidateColor(string? color)
        {
            return color != null && Regex.IsMatch(color, "^#([a-fA-F0-9]{6})$");
        }

        public bool ValidateEanCode(string? code)
        {
            return code != null && Regex.IsMatch(code, "^[0-9]{13}$");
        }

        public bool ValidateFrameSize(int? size)
        {
            return size != null && size > 0 && size < 100;
        }

        public bool ValidateLink(string? link)
        {
            return link != null && Regex.IsMatch(link, "^http://|https://*");
        }

        public bool ValidateModelName(string? name)
        {
            return name != null && Regex.IsMatch(name, "^[a-zA-Z0-9żźćńółęąśŻŹĆĄŚĘŁÓŃ. _\\-]{4,50}$");
        }

        public bool ValidatePrice(int? price)
        {
            return price != null && price > 100 && price < 100000;
        }

        public bool ValidateProductCode(string? code)
        {
            return code != null && Regex.IsMatch(code, "^[a-zA-Z0-9_\\-]{4,30}$");
        }

        public bool ValidateWheelSize(int? size)
        {
            return size != null && size > 10 && size < 30;
        }
    }
}
