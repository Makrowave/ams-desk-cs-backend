using System;
using System.Collections.Generic;

namespace ams_desk_cs_backend.BikeService.Models;

public partial class Place
{
    public short PlaceId { get; set; }

    public required string PlaceName { get; set; }
    public virtual ICollection<Bike> Bikes { get; set; } = new List<Bike>();
}
