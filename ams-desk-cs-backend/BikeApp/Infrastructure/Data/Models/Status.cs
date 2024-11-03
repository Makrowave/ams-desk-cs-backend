using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ams_desk_cs_backend.BikeApp.Infrastructure.Data.Models;

public partial class Status
{
    public short StatusId { get; set; }
    public required string StatusName { get; set; }
    public required string HexCode { get; set; }
    public virtual ICollection<Bike> Bikes { get; set; } = new List<Bike>();
}
