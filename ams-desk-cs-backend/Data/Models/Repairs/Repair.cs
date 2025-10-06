using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ams_desk_cs_backend.Data.Models.Repairs;

[Table("repairs")]
public class Repair
{
    [Key]
    [Column("repair_id")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int RepairId { get; set; }

    [Column("phone_number")]
    [MaxLength(9)]
    public required string PhoneNumber { get; set; }

    [Column("bike_name")]
    [MaxLength(40)]
    public required string BikeName { get; set; }

    [Column("issue")]
    [MaxLength(200)]
    public required string Issue { get; set; }

    [Column("arrival_date")]
    public required DateOnly ArrivalDate { get; set; }

    [Column("collection_date")]
    public DateOnly? CollectionDate { get; set; }

    [Column("take_in_employee_id")]
    public short TakeInEmployeeId { get; set; }

    [Column("repair_employee_id")]
    public short? RepairEmployeeId { get; set; }

    [Column("collection_employee_id")]
    public short? CollectionEmployeeId { get; set; }

    [Column("discount")]
    public float Discount { get; set; }

    [Column("additional_costs")]
    public float AdditionalCosts { get; set; }

    [Column("status_id")]
    public short StatusId { get; set; }

    [Column("place_id")]
    public short PlaceId { get; set; }

    [Column("note")]
    [MaxLength(1000)]
    public string? Note { get; set; }

    [ForeignKey(nameof(StatusId))]
    [InverseProperty(nameof(RepairStatus.Repairs))]
    public virtual RepairStatus? Status { get; set; }

    [ForeignKey(nameof(CollectionEmployeeId))]
    [InverseProperty(nameof(Employee.CollectionRepairs))]
    public virtual Employee? CollectionEmployee { get; set; }

    [ForeignKey(nameof(RepairEmployeeId))]
    [InverseProperty(nameof(Employee.RepairRepairs))]
    public virtual Employee? RepairEmployee { get; set; }

    [ForeignKey(nameof(TakeInEmployeeId))]
    [InverseProperty(nameof(Employee.TakeInRepairs))]
    public virtual Employee? TakeInEmployee { get; set; }

    [ForeignKey(nameof(PlaceId))]
    [InverseProperty(nameof(Place.Repairs))]
    public virtual Place? Place { get; set; }

    public virtual ICollection<ServiceDone> Services { get; set; } = new List<ServiceDone>();
    public virtual ICollection<PartUsed> Parts { get; set; } = new List<PartUsed>();

    public float GetTotalPrice()
    {
        float totalPrice = 0;
        Services.ToList().ForEach(service => totalPrice += service.Price);
        Parts.ToList().ForEach(part => totalPrice += part.Price);
        totalPrice += AdditionalCosts;
        totalPrice -= Discount;
        return totalPrice;
    }
}
