namespace ams_desk_cs_backend.Deliveries.Dtos;

public record NotAssignedInvoiceDto
{
    public int Id { get; init; }
    public string Name { get; init; } = null!;
}