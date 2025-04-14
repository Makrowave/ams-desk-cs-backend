namespace ams_desk_cs_backend.Data.Models.Repairs;

public class ServiceCategory
{
    public short ServiceCategoryId { get; set; }
    public string ServiceCategoryName { get; set; } = null!;
    public virtual ICollection<Service> Services { get; set; } = [];
}