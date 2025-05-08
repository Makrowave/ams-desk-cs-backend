

using System.ComponentModel.DataAnnotations;
using ams_desk_cs_backend.Data.Models;
using ams_desk_cs_backend.Shared;

namespace ams_desk_cs_backend.Places.Dtos;

public partial class PlaceDto
{
    public PlaceDto() { }

    public PlaceDto(Place place)
    {
        PlaceId = place.PlaceId;
        PlaceName = place.PlaceName;
        IsStorage = place.IsStorage;
    }
    public short? PlaceId { get; set; }

    [Required]
    [RegularExpression(Regexes.PolishText)]
    [MaxLength(40)]
    public string PlaceName { get; set; } = null!;
    [Required]
    public bool IsStorage { get; set; }
}
