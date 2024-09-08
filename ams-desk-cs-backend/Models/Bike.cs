using System;
using System.Collections.Generic;

namespace ams_desk_cs_backend.Models;

public partial class Bike
{
    public int BikeId { get; set; }

    public int ModelId { get; set; }

    public short PlaceId { get; set; }

    public short StatusId { get; set; }

    public DateOnly InsertionDate { get; set; }

    public DateOnly? SaleDate { get; set; }

    public int? SalePrice { get; set; }

    public virtual Model? Model { get; set; }

    public virtual Place? Place { get; set; }

    public virtual Status? Status { get; set; }
}
