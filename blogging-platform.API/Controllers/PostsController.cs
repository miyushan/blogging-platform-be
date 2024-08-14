using Microsoft.AspNetCore.Mvc;
using blogging_platform.API.Models.Domain;
using blogging_platform.API.Models.DTO;
using blogging_platform.API.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using blogging_platform.API.Validations;
using System.Xml.Linq;
using Microsoft.EntityFrameworkCore;

namespace blogging_platform.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostsController : ControllerBase
    {
        private readonly BloggingPlatformDbContext _dbContext;

        public PostsController(BloggingPlatformDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<GetPostResDto>>> GetAllPosts([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var posts = await _dbContext.Posts
                    .Skip((page-1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                var postsDto = new List<GetPostResDto>();
                foreach (var post in posts)
                {
                    var author = _dbContext.Users.Find(post.UserId);
                    var category = _dbContext.Categories.Find(post.CategoryId);

                    if (author == null || category == null)
                    {
                        continue;
                    }

                    var comments = _dbContext.Comments
                        .Where(c => c.PostId == post.PostId)
                        .Select(c => new GetCommentDto
                    {
                        CommentId = c.CommentId,
                        Content = c.Content,
                        CreatedAt = c.CreatedAt
                    }).ToList();

                    postsDto.Add(new GetPostResDto()
                    {
                        PostId = post.PostId,
                        Title = post.Title,
                        Content = post.Content,
                        Comments = comments,
                        Author = new GetAuthorDto()
                        {
                            Id = author.UserId,
                            FirstName = author.FirstName,
                            LastName = author.LastName,

                        },
                        Category = new GetCategoryDto()
                        {
                            Id = category.CategoryId,
                            Name = category.Name
                        }
                    });
                }
                return Ok(postsDto);
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{id:Guid}")]
        public async Task<ActionResult<GetPostResDto>> GetPostById(Guid id)
        {
            var post = await _dbContext.Posts.FindAsync(id);
            if (post == null)
            {
                return NotFound();
            }

            var author = await _dbContext.Users.FindAsync(post.UserId);
            var category = await _dbContext.Categories.FindAsync(post.CategoryId);
            if (author == null || category == null)
            {
                return NotFound();
            }

            var comments = await _dbContext.Comments
                .Where(c => c.PostId == post.PostId)
                .Select(c => new GetCommentDto
                {
                    CommentId = c.CommentId,
                    Content = c.Content,
                    CreatedAt = c.CreatedAt
                }).ToListAsync();

            var postDto = new GetPostResDto{
                PostId = post.PostId,
                Title = post.Title,
                Content = post.Content,
                Comments = comments,
                Author = new GetAuthorDto()
                {
                    Id = author.UserId,
                    FirstName = author.FirstName,
                    LastName = author.LastName,
                },
                Category = new GetCategoryDto()
                {
                    Id = category.CategoryId,
                    Name = category.Name
                }
            };
            return Ok(postDto);
        }
    
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost]
        public async Task<ActionResult<GetPostResDto>> CreatePost(CreatePostReqDto post)
        {
            var validator = new CreatePostReqValidator();
            var results = validator.Validate(post);
            if (!results.IsValid)
            {
                return BadRequest(results.Errors);
            }

            var author = await _dbContext.Users.FindAsync(post.UserId);
            var category = await _dbContext.Categories.FindAsync(post.CategoryId);
            if (author == null || category == null) {
                return NotFound();
            }

            var newPost = new Post
            {
                PostId = Guid.NewGuid(),
                Title = post.Title,
                Content = post.Content,
                UserId = post.UserId,
                CategoryId = post.CategoryId
            };

            await _dbContext.Posts.AddAsync(newPost);
            await _dbContext.SaveChangesAsync();

            var newPostDto = new GetPostResDto{
                PostId = newPost.PostId,
                Title = newPost.Title,
                Content = newPost.Content,
                Comments = new List<GetCommentDto>(),
                Author = new GetAuthorDto()
                {
                    Id = author.UserId,
                    FirstName = author.FirstName,
                    LastName = author.LastName,
                },
                Category = new GetCategoryDto()
                {
                    Id = category.CategoryId,
                    Name = category.Name
                }
            };

            return CreatedAtAction(nameof(GetPostById), new {id=newPostDto.PostId}, newPostDto);
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPut("{postId:Guid}")]
        public async Task<ActionResult<GetPostResDto>> EditPost([FromRoute] Guid postId, [FromBody] EditPostReqDto body)
        {
            var validator = new EditPostReqValidator();
            var results = validator.Validate(body);
            if (!results.IsValid)
            {
                return BadRequest(results.Errors);
            }

            var post = await _dbContext.Posts.FindAsync(postId);
            var category = await _dbContext.Categories.FindAsync(body.CategoryId);
            if (post == null || category == null)
            {
                return NotFound();
            }
            var author = await _dbContext.Users.FindAsync(post.UserId);
            if (author == null)
            {
                return NotFound();
            }

            post.Title = body.Title;
            post.Content = body.Content;
            post.CategoryId = body.CategoryId;

            await _dbContext.SaveChangesAsync();

            var comments = await _dbContext.Comments
                .Where(c => c.PostId == post.PostId)
                .Select(c => new GetCommentDto
                {
                    CommentId = c.CommentId,
                    Content = c.Content,
                    CreatedAt = c.CreatedAt
                })
                .ToListAsync();

            var updatedPostDto = new GetPostResDto
            {
                PostId = post.PostId,
                Title = post.Title,
                Content = post.Content,
                Comments = comments,
                Author = new GetAuthorDto()
                {
                    Id = author.UserId,
                    FirstName = author.FirstName,
                    LastName = author.LastName,
                },
                Category = new GetCategoryDto()
                {
                    Id = category.CategoryId,
                    Name = category.Name
                }
            };

            return CreatedAtAction(nameof(GetPostById), new { id = postId }, updatedPostDto);
        }


        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpDelete("{postId:Guid}")]
        public async Task<IActionResult> DeletePost([FromRoute] Guid postId, [FromBody] DeletePostReqDto body)
        {
            var validator = new DeletePostReqValidator();
            var results = validator.Validate(body);
            if (!results.IsValid)
            {
                return BadRequest(results.Errors);
            }

            var post = await _dbContext.Posts.FindAsync(postId);
            if(post == null)
            {
                return NotFound();
            }

            if(post.UserId != body.AuthorId)
            {
                return Forbid();
            }

            _dbContext.Remove(post);
            await _dbContext.SaveChangesAsync();

            return NoContent();
        }
    }
}
