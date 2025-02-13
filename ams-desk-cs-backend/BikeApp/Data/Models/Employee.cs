using ams_desk_cs_backend.BikeApp.Data.Models.Repairs;

namespace ams_desk_cs_backend.BikeApp.Data.Models
{
    public partial class Employee
    {
        public short EmployeeId { get; set; }
        public required string EmployeeName { get; set; }
        public required short EmployeesOrder { get; set; }
        public ICollection<Bike> Bikes { get; set; } = [];
        public ICollection<Repair> RepairRepairs { get; set; } = [];
        public ICollection<Repair> CollectionRepairs { get; set; } = [];
    }
}
