using Microsoft.AspNetCore.Mvc;
using blogging_platform.API.Data;
using blogging_platform.API.Models.DTO;
using blogging_platform.API.Models.Domain;
using blogging_platform.API.Validations;

namespace blogging_platform.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentController : ControllerBase
    {
        private readonly BloggingPlatformDbContext _dbContext;

        public CommentController(BloggingPlatformDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateCommentReqDto comment)
        {
            var validator = new CreateCommentReqValidator();
            var results = validator.Validate(comment);
            if (!results.IsValid)
            {
                return BadRequest(results.Errors);
            }

            var post = await _dbContext.Posts.FindAsync(comment.PostId);
            if (post == null)
            {
                return NotFound();
            }

            var newComment = new Comment
            {
                CommentId = Guid.NewGuid(),
                Content = comment.Content,
                PostId = comment.PostId,
                CreatedAt = DateTime.UtcNow,
            };

            await _dbContext.Comments.AddAsync(newComment);
            await _dbContext.SaveChangesAsync();

            var newCommentDto = new GetCommentResDto
            {
                CommentId = newComment.CommentId,
                Content = newComment.Content,
                PostId = newComment.PostId,
                CreatedAt = newComment.CreatedAt
            };

            return Ok(newCommentDto);
        }
    }
}
