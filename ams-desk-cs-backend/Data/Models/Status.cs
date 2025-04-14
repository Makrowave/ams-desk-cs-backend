namespace ams_desk_cs_backend.Data.Models;

public partial class Status
{
    public short StatusId { get; set; }
    public required string StatusName { get; set; }
    public required string HexCode { get; set; }
    public required short StatusesOrder { get; set; }
    public virtual ICollection<Bike> Bikes { get; set; } = new List<Bike>();
}
