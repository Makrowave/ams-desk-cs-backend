using System;
using System.Collections.Generic;

namespace ams_desk_cs_backend.BikeService.Models;

public partial class Status
{
    public short StatusId { get; set; }

    public string StatusName { get; set; } = null!;

    public virtual ICollection<Bike> Bikes { get; set; } = new List<Bike>();
}
