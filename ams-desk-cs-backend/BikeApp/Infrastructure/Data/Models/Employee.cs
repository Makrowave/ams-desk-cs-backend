using ams_desk_cs_backend.BikeApp.Infrastructure.Data.Models.Repairs;

namespace ams_desk_cs_backend.BikeApp.Infrastructure.Data.Models
{
    public partial class Employee
    {
        public short EmployeeId { get; set; }
        public required string EmployeeName { get; set; }
        public required short EmployeesOrder { get; set; }
        public ICollection<Bike> Bikes { get; set; } = [];
        public ICollection<Repair> Repairs { get; set; } = [];
    }
}
