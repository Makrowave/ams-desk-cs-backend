using System;
using System.Collections.Generic;

namespace ams_desk_cs_backend.BikeService.Models;

public partial class Model
{
    public int ModelId { get; set; }

    public required string ProductCode { get; set; }

    public string? EanCode { get; set; }
    public required string ModelName { get; set; }
    public short FrameSize { get; set; }

    public bool IsWoman { get; set; }

    public short WheelSize { get; set; }

    public short ManufacturerId { get; set; }
    public short ColorId { get; set; }
    public short CategoryId { get; set; }
    public string? PrimaryColor { get; set; }
    public string? SecondaryColor { get; set; }

    public int Price { get; set; }

    public bool IsElectric { get; set; }
    public string? Link { get; set; }

    public virtual ICollection<Bike> Bikes { get; set; } = new List<Bike>();

    public virtual Manufacturer? Manufacturer { get; set; }
    public virtual Color? Color { get; set; }
    public virtual Category? Category { get; set; }
}
