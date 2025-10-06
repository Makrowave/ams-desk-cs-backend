using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ams_desk_cs_backend.Data.Models.Repairs;

[Index(nameof(ServiceCategoryName), IsUnique = true)]
[Table("service_categories")]
public class ServiceCategory
{
    [Key]
    [Column("service_category_id")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public short ServiceCategoryId { get; set; }

    [Column("service_name")]
    [MaxLength(30)]
    public required string ServiceCategoryName { get; set; }

    public virtual ICollection<Service> Services { get; set; } = new List<Service>();
}