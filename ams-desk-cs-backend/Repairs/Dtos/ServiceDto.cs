using System.ComponentModel.DataAnnotations;
using ams_desk_cs_backend.Data.Models.Repairs;
using ams_desk_cs_backend.Shared;

namespace ams_desk_cs_backend.Repairs.Dtos;

public class ServiceDto
{
    public ServiceDto(Service service)
    {
        ServiceId = service.Id;
        ServiceName = service.Name;
        Price = service.Price;
        ServiceCategoryId = service.ServiceCategoryId;
        ServiceCategory = service.ServiceCategory;
    }
    public ServiceDto() {}
    public short? ServiceId { get; set; }
    [Required]
    [RegularExpression(Regexes.PolishText)]
    [MaxLength(40)]
    public string ServiceName { get; set; } = null!;
    [Required]
    [Range(0, float.MaxValue)]
    public float Price { get; set; }
    [Required]
    public short ServiceCategoryId { get; set; }
    
    public ServiceCategory? ServiceCategory { get; set; }
}