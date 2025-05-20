namespace ams_desk_cs_backend.Data.Models.Messages;

public class Post
{
    public int Id { get; init; }
    public short PlaceId { get; init; }
    public string Content { get; set; } = null!;
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; init; }
    public short UserId { get; set; }
    public bool IsResolved { get; set; }
    public bool IsArchived { get; set; }
    
    // Navigation properties
    public List<Response> Responses { get; set; } = [];
    public Place? Place { get; set; }
}