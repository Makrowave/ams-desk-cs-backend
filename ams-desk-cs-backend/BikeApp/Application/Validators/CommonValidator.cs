using ams_desk_cs_backend.BikeApp.Application.Interfaces.Validators;
using System.Text.RegularExpressions;
using System.Xml.Linq;

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

        public bool ValidateEmployeeName(string name)
        {
            return Regex.IsMatch(name, "^[A-ZŻÓŁĆĘŚĄŹŃa-zżółćęśąźń .]{1,16}$");
        }
        public bool ValidatePassword(string password)
        {
            return Regex.IsMatch(password, "^[A-ZŻÓŁĆĘŚĄŹŃa-zżółćęśąźń0-9!@#$%^&*()]{8,}$");
        }
    }
}
