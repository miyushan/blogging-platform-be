
namespace blogging_platform.API.Models.Domain;

public class Comment
{
    public Guid CommentId { get; set; }
    public string Content { get; set; } = string.Empty;
    public Guid PostId { get; set; }

    // Navigation properties
    public Post Post { get; set; } = null!;
}
