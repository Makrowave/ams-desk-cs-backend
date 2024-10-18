using System;
using System.Collections.Generic;

namespace ams_desk_cs_backend.BikeService.Models;

public partial class Manufacturer
{
    public short ManufacturerId { get; set; }

    public required string ManufacturerName { get; set; }

    public virtual ICollection<Model> Models { get; set; } = new List<Model>();
}
