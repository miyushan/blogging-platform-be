using System;

namespace blogging_platform.API.Models.Domain;

public class Post
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public Guid UserId { get; set; }
    public Guid CategoryId { get; set; }


    // Navigation properties
    public User User { get; set; } = new User();
    public Category Category { get; set; } = new Category();

}
