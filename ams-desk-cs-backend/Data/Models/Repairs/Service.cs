using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using ams_desk_cs_backend.Repairs.Dtos;

namespace ams_desk_cs_backend.Data.Models.Repairs;

[Table("services")]
public class Service
{
    public Service() { }
    public Service(ServiceDto dto, short id) : this(dto)
    {
        Id = id;
    }
    public Service(ServiceDto dto)
    {
        Name = dto.Name;
        Price = dto.Price;
        ServiceCategoryId = dto.ServiceCategoryId;
    }

    [Key]
    [Column("service_id")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public short Id { get; set; }

    [Column("service_name")]
    [MaxLength(50)]
    public string Name { get; set; }

    [Column("price")]
    public float Price { get; set; }

    [Column("service_category_id")]
    public short ServiceCategoryId { get; set; }

    [ForeignKey(nameof(ServiceCategoryId))]
    [InverseProperty(nameof(ServiceCategory.Services))]
    public virtual ServiceCategory? ServiceCategory { get; set; }

    public virtual ICollection<ServiceDone> ServicesDone { get; set; } = new List<ServiceDone>();
}