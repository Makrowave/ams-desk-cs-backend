using System.ComponentModel.DataAnnotations;
using ams_desk_cs_backend.Repairs.Dtos;
using ams_desk_cs_backend.Shared;

namespace ams_desk_cs_backend.Data.Models.Repairs;

public class Service
{
    public Service() { }
    public Service(ServiceDto dto, short id) : this(dto)
    {
        ServiceId = id;
    }
    public Service(ServiceDto dto)
    {
        ServiceName = dto.ServiceName;
        Price = dto.Price;
        ServiceCategoryId = dto.ServiceCategoryId;
    }
    public short ServiceId { get; set; }
    public string ServiceName { get; set; } = null!;
    public float Price { get; set; }
    public short ServiceCategoryId { get; set; }
    public virtual ServiceCategory? ServiceCategory { get; set; }
    public virtual ICollection<ServiceDone> ServicesDone { get; set; } = [];
}