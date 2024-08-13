

namespace blogging_platform.API.Models.DTO
{
    public class GetAuthorDto
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;

    }

    public class GetCategoryDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    public class GetPostResDto
    {
        public Guid PostId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public GetAuthorDto Author { get; set; } = null!;
        public GetCategoryDto Category { get; set; } = null!;
    }
}