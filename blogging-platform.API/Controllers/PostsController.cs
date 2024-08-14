using Microsoft.AspNetCore.Mvc;
using blogging_platform.API.Models.Domain;
using blogging_platform.API.Models.DTO;
using blogging_platform.API.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using blogging_platform.API.Validations;
using System.Xml.Linq;

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
        public IActionResult GetAllPosts()
        {
            var posts = _dbContext.Posts.ToList();

            var postsDto = new List<GetPostResDto>();
            foreach (var post in posts)
            {   
                var author = _dbContext.Users.Find(post.UserId);
                var category = _dbContext.Categories.Find(post.CategoryId);

                if(author == null || category == null)
                {
                    continue;
                }

                var comments = _dbContext.Comments.Where(c => c.PostId == post.PostId).Select(c => new GetCommentDto
                {
                    CommentId = c.CommentId,
                    Content = c.Content,
                    CreatedAt = c.CreatedAt
                }).ToList();

                postsDto.Add(new GetPostResDto(){
                    PostId = post.PostId,
                    Title = post.Title,
                    Content = post.Content,
                    Comments = comments,
                    Author =  new GetAuthorDto()
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

        [HttpGet]
        [Route("{id:Guid}")]
        public IActionResult GetPostById([FromRoute] Guid id)
        {
            var post = _dbContext.Posts.Find(id);
            var author = _dbContext.Users.Find(post?.UserId);
            var category = _dbContext.Categories.Find(post?.CategoryId);
            if (post == null || author == null || category == null)
            {
                return NotFound();
            }

            var comments = _dbContext.Comments.Where(c => c.PostId == post.PostId).Select(c => new GetCommentDto
            {
                CommentId = c.CommentId,
                Content = c.Content,
                CreatedAt = c.CreatedAt
            }).ToList();

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
        public IActionResult CreatePost([FromBody] CreatePostReqDto post)
        {
            var validator = new CreatePostReqValidator();
            var results = validator.Validate(post);
            if (!results.IsValid)
            {
                return BadRequest(results.Errors);
            }

            var author = _dbContext.Users.Find(post.UserId);
            var category = _dbContext.Categories.Find(post.CategoryId);
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

            _dbContext.Posts.Add(newPost);
            _dbContext.SaveChanges();

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
        [HttpDelete]
        [Route("{postId:Guid}")]
        public IActionResult DeletePost([FromRoute] Guid postId, [FromBody] DeletePostReqDto body)
        {
            var validator = new DeletePostReqValidator();
            var results = validator.Validate(body);
            if (!results.IsValid)
            {
                return BadRequest(results.Errors);
            }

            var post = _dbContext.Posts.Find(postId);
            if(post == null)
            {
                return NotFound();
            }

            if(post.UserId != body.AuthorId)
            {
                return Forbid();
            }

            _dbContext.Remove(post);
            _dbContext.SaveChanges();
            return Ok();
        }
    }
}
