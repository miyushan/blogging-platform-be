using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using blogging_platform.API.Models.Domain;
using blogging_platform.API.Models.DTO;
using blogging_platform.API.Data;

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

        // GET: All posts
        // GET: baseUrl/api/Posts
        [HttpGet]
        public IActionResult GetAll()
        {
            // Get data from database
            var posts = dbContext.Posts.ToList();

            // Map domain models to DTOs
            var postDto = new List<PostDto>();
            
            foreach (var post in posts)
            {   
                postDto.Add(new PostDto(){
                    Id = post.Id,
                    Title = post.Title,
                    Content = post.Content,
                    UserId = post.UserId,
                    CategoryId = post.CategoryId
                });
            }

            // Return DTOs
            return Ok(posts);
        }

        // GET: Single post     
        // GET: baseUrl/api/Posts/:id
        [HttpGet]
        [Route("{id:Guid}")]
        public IActionResult GetById([FromRoute] Guid id)
        {
            // Get data from database
            var post = dbContext.Posts.Find(id);
            if (post == null)
            {
                return NotFound();
            }

            // Map domain models to DTOs
            var postDto = new PostDto{
                Id = post.Id,
                Title = post.Title,
                Content = post.Content,
                UserId = post.UserId,
                CategoryId = post.CategoryId
            };
            return Ok(postDto);
        }
    
        // POST: Create new post
        // POST: baseUrl/api/Posts
        [HttpPost]
        public IActionResult Create([FromBody] CreatePostReqDto post)
        {
            // Map DTO to domain model
            var newPost = new Post
            {
                Id = Guid.NewGuid(),
                Title = post.Title,
                Content = post.Content,
                UserId = post.UserId,
                CategoryId = post.CategoryId
            };

            // Add domain model to database
            dbContext.Posts.Add(newPost);
            dbContext.SaveChanges();

            // Return DTO
            var newPostDto = new PostDto{
                Id = newPost.Id,
                Title = newPost.Title,
                Content = newPost.Content,
                UserId = newPost.UserId,
                CategoryId = newPost.CategoryId
            };

            return CreatedAtAction(nameof(GetById), new {id=newPostDto.Id}, newPostDto);
        }
    
    }
}
