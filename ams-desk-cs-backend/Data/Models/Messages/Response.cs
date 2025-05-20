namespace ams_desk_cs_backend.Data.Models.Messages;

public class Response
{
    public int Id { get; init; }
    public string Content { get; init; } = null!;
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; init; }
    public short UserId { get; init; }
    public Post? Post { get; set; }
}