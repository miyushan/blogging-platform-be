namespace blogging_platform.API.Models.DTO
{
    public class CreateCommentReqDto
    {
        public string Content { get; set; } = string.Empty;
        public Guid PostId { get; set; }
    }
}
