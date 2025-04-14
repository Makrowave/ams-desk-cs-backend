using ams_desk_cs_backend.Data.Models.Repairs;

namespace ams_desk_cs_backend.Data.Models;

public partial class Place
{
    public short PlaceId { get; set; }

    public required string PlaceName { get; set; }
    public required short PlacesOrder { get; set; }
    public virtual ICollection<Bike> Bikes { get; set; } = [];
    public virtual ICollection<Repairs.Repair> Repairs { get; set; } = [];
}
