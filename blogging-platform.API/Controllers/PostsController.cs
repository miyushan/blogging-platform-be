using Microsoft.AspNetCore.Mvc;
using blogging_platform.API.Models.Domain;
using blogging_platform.API.Models.DTO;
using blogging_platform.API.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using blogging_platform.API.Validations;

namespace blogging_platform.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostsController : ControllerBase
    {
        private readonly BloggingPlatformDbContext dbContext;

        public PostsController(BloggingPlatformDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var posts = dbContext.Posts.ToList();

            var postsDto = new List<GetPostResDto>();
            foreach (var post in posts)
            {   
                postsDto.Add(new GetPostResDto(){
                    Id = post.PostId,
                    Title = post.Title,
                    Content = post.Content,
                    UserId = post.UserId,
                    CategoryId = post.CategoryId
                });
            }
            return Ok(postsDto);
        }

        [HttpGet]
        [Route("{id:Guid}")]
        public IActionResult GetById([FromRoute] Guid id)
        {
            var post = dbContext.Posts.Find(id);
            if (post == null)
            {
                return NotFound();
            }

            var postDto = new GetPostResDto{
                Id = post.PostId,
                Title = post.Title,
                Content = post.Content,
                UserId = post.UserId,
                CategoryId = post.CategoryId
            };
            return Ok(postDto);
        }
    

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost]
        public IActionResult Create([FromBody] CreatePostReqDto post)
        {
            var validator = new CreatePostReqValidator();
            var results = validator.Validate(post);
            if (!results.IsValid)
            {
                return BadRequest(results.Errors);
            }

            var newPost = new Post
            {
                PostId = Guid.NewGuid(),
                Title = post.Title,
                Content = post.Content,
                UserId = post.UserId,
                CategoryId = post.CategoryId
            };

            dbContext.Posts.Add(newPost);
            dbContext.SaveChanges();

            var newPostDto = new GetPostResDto{
                Id = newPost.PostId,
                Title = newPost.Title,
                Content = newPost.Content,
                UserId = newPost.UserId,
                CategoryId = newPost.CategoryId
            };

            return CreatedAtAction(nameof(GetById), new {id=newPostDto.Id}, newPostDto);
        }
    
    }
}
