using ams_desk_cs_backend.BikeApp.Application.Interfaces.Validators;
using System.Text.RegularExpressions;

namespace ams_desk_cs_backend.BikeApp.Application.Validators
{
    public class CommonValidator : ICommonValidator
    {
        public bool Validate16CharName(string name)
        {
            return Regex.IsMatch(name, "^[A-ZŻÓŁĆĘŚĄŹŃ][a-zżółćęśąźń]{1,15}$");
        }

        public bool Validate16CharNameAnyCase(string name)
        {
            return Regex.IsMatch(name, "^[A-ZŻÓŁĆĘŚĄŹŃa-zżółćęśąźń 0-9-]{1,16}$");
        }

        public bool ValidateColor(string color)
        {
            return Regex.IsMatch(color, "^#[A-Fa-f0-9]{6}$");
        }
    }
}
