using System;
using System.Collections.Generic;

namespace ams_desk_cs_backend.BikeService.Models;

public partial class Model
{
    public int ModelId { get; set; }

    public string ProductCode { get; set; } = null!;

    public string EanCode { get; set; } = null!;
    public string ModelName { get; set; } = null!;
    public short FrameSize { get; set; }

    public bool IsWoman { get; set; }

    public short WheelSize { get; set; }

    public short ManufacturerId { get; set; }

    public int Price { get; set; }

    public bool IsElectric { get; set; }

    public virtual ICollection<Bike> Bikes { get; set; } = new List<Bike>();

    public virtual Manufacturer? Manufacturer { get; set; }

}
