using ams_desk_cs_backend.BikeApp.Data.Models;

namespace ams_desk_cs_backend.BikeApp.Data.Models.Repairs
{
    public class Repair
    {
        //Arrival - when client leaves bike at the shop
        //Collection - when client collects bike
        //Repair Employee - employee that did most repairs on bike. If other employee did some job - use notes
        //Collection Employee - employee that returned bike to the client
        public int RepairId { get; set; }
        public string PhoneNumber { get; set; } = null!;
        public string BikeName { get; set; } = null!;
        public string Issue { get; set; } = null!;
        public DateOnly ArrivalDate { get; set; }
        public DateOnly? CollectionDate { get; set; }
        public short? RepairEmployeeId { get; set; }
        public short? CollectionEmployeeId { get; set; }
        public float Discount { get; set; } = 0;
        public float AdditionalCosts { get; set; } = 0;
        public short StatusId { get; set; }
        public string? Note { get; set; }

        //Relations
        public virtual RepairStatus? Status { get; set; }
        public virtual Employee? CollectionEmployee { get; set; }
        public virtual Employee? RepairEmployee { get; set; }
        public virtual ICollection<ServiceDone> Services { get; set; } = [];
        public virtual ICollection<PartUsed> Parts { get; set; } = [];

    }
}
