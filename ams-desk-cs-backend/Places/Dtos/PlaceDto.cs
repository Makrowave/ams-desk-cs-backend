

using System.ComponentModel.DataAnnotations;
using ams_desk_cs_backend.Data.Models;
using ams_desk_cs_backend.Shared;

namespace ams_desk_cs_backend.Places.Dtos;

public partial class PlaceDto
{
    public PlaceDto() { }

    public PlaceDto(Place place)
    {
        Id = place.Id;
        Name = place.Name;
        IsStorage = place.IsStorage;
    }
    public short? Id { get; set; }

    [Required]
    [RegularExpression(Regexes.PolishText)]
    [MaxLength(40)]
    public string Name { get; set; } = null!;
    [Required]
    public bool IsStorage { get; set; }
}
