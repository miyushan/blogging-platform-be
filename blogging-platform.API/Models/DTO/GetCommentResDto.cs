namespace blogging_platform.API.Models.DTO
{
    public class GetCommentResDto
    {
        public Guid CommentId { get; set; }
        public Guid PostId { get; set; }
        public string Content { get; set; } = string.Empty;
        public DateTime CreatedAt{ get; set; }
    }
}
