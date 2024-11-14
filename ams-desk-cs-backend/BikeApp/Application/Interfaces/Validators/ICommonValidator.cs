namespace ams_desk_cs_backend.BikeApp.Application.Interfaces.Validators
{
    public interface ICommonValidator
    {
        bool Validate16CharName(string name);
        bool Validate16CharNameAnyCase(string name);
        bool ValidateColor(string name);
    }
}
