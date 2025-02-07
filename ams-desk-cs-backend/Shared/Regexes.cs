using System.Text.RegularExpressions;

namespace ams_desk_cs_backend.Shared
{
    public struct Regexes
    {
        public const string Name16 = "^[A-ZŻÓŁĆĘŚĄŹŃ][a-zżółćęśąźń]{1,15}$";
        public const string Color = "^#[A-Fa-f0-9]{6}$";
        public const string NameAnyCase16 = "^[A-ZŻÓŁĆĘŚĄŹŃa-zżółćęśąźń 0-9-]{1,16}$";
        public const string EmployeeName = "^[A-ZŻÓŁĆĘŚĄŹŃa-zżółćęśąźń .]{1,16}$";
        public const string Password = "^[A-ZŻÓŁĆĘŚĄŹŃa-zżółćęśąźń0-9!@#$%^&*()]{8,}$";
        public const string EanCode = "^[0-9]{13}$";
        public const string Link = "^http://|https://*";
        public const string ModelName = "^[a-zA-Z0-9żźćńółęąśŻŹĆĄŚĘŁÓŃ. _\\-]{4,50}$";
        public const string ProductCode = "^[a-zA-Z0-9_\\-]{4,30}$";
    }
}
