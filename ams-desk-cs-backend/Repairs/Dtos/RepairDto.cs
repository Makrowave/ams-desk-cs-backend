using System.ComponentModel.DataAnnotations;
using ams_desk_cs_backend.Data.Models.Repairs;
using ams_desk_cs_backend.Shared;

namespace ams_desk_cs_backend.Repairs.Dtos;

public class RepairDto
{
    public RepairDto() { }
    public RepairDto(Repair repair)
    {
        RepairId = repair.Id;
        PhoneNumber = repair.PhoneNumber;
        BikeName = repair.BikeName;
        Issue = repair.Issue;
        ArrivalDate = repair.ArrivalDate;
        CollectionDate = repair.CollectionDate;
        RepairEmployeeId = repair.RepairEmployeeId;
        CollectionEmployeeId = repair.CollectionEmployeeId;
        TakeInEmployeeId = repair.TakeInEmployeeId;
        Discount = repair.Discount;
        AdditionalCosts = repair.AdditionalCosts;
        StatusId = (short)repair.StatusId;
        PlaceId = repair.PlaceId;
        Note = repair.Note;
        Status = repair.Status;
        TakeInEmployeeName = repair.TakeInEmployee!.Name;
        RepairEmployeeName = repair.RepairEmployee?.Name;
        CollectionEmployeeName = repair.CollectionEmployee?.Name;
        Services = repair.Services;
        Parts = repair.Parts;
        foreach (var service in Services)
        {
            service.Repair = null;
        }
        foreach (var part in Parts)
        {
            part.Repair = null;
        }
        Status?.Repairs?.Clear();
    }
    [Required]
    public int RepairId { get; set; }
    [Required]
    public string PhoneNumber { get; set; } = null!;
    [Required]
    public string BikeName { get; set; } = null!;
    [Required]
    [RegularExpression(Regexes.PolishText)]
    [MaxLength(200)]
    public string Issue { get; set; } = null!;
    [Required]
    public DateOnly ArrivalDate { get; set; }
    public DateOnly? CollectionDate { get; set; }
    public short TakeInEmployeeId { get; set; }
    public string TakeInEmployeeName { get; set; }
    public short? RepairEmployeeId { get; set; }
    public short? CollectionEmployeeId { get; set; }
    [Required]
    [Range(0, float.MaxValue)]
    public float Discount { get; set; } = 0;
    [Required]
    [Range(0, float.MaxValue)]
    public float AdditionalCosts { get; set; } = 0;
    [Required]
    public short StatusId { get; set; }
    [Required]
    public short PlaceId { get; set; }
    [RegularExpression(Regexes.PolishText)]
    [MaxLength(1000)]
    public string? Note { get; set; }
    public RepairStatus? Status { get; set; }
    //Names without validation because they only come from backend - they don't return
    public string? RepairEmployeeName { get; set; }
    public string? CollectionEmployeeName { get; set; }
    public virtual ICollection<ServiceDone> Services { get; set; } = [];
    public virtual ICollection<PartUsed> Parts { get; set; } = [];
}