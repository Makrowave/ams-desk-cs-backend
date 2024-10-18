namespace ams_desk_cs_backend.BikeService.Models
{
    public partial class Employee
    {
        public short EmployeeId { get; set; }
        public required string EmployeeName { get; set; }
        public ICollection<Bike> Bikes { get; set; } = new List<Bike>();
    }
}
