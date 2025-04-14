using ams_desk_cs_backend.Data.Models.Repairs;

namespace ams_desk_cs_backend.Data.Models;

public partial class Employee
{
    public short EmployeeId { get; set; }
    public required string EmployeeName { get; set; }
    public required short EmployeesOrder { get; set; }
    public ICollection<Bike> Bikes { get; set; } = [];
    public ICollection<Repairs.Repair> RepairRepairs { get; set; } = [];
    public ICollection<Repairs.Repair> CollectionRepairs { get; set; } = [];
    public ICollection<Repairs.Repair> TakeInRepairs { get; set; } = [];
}