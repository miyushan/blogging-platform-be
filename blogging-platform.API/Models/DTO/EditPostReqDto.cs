namespace blogging_platform.API.Models.DTO
{
    public class EditPostReqDto
    {
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public Guid CategoryId { get; set; }
    }
}
