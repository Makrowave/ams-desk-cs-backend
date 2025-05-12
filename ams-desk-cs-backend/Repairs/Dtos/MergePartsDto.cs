using ams_desk_cs_backend.Data.Models.Repairs;

namespace ams_desk_cs_backend.Repairs.Dtos;

public class MergePartsDto
{
    public int Id1 { get; set; }
    public int Id2 { get; set; }
    public Part Part { get; set; }
}