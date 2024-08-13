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
        private readonly BloggingPlatformDbContext _dbContext;

        public PostsController(BloggingPlatformDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        public IActionResult GetAll()
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

                postsDto.Add(new GetPostResDto(){
                    PostId = post.PostId,
                    Title = post.Title,
                    Content = post.Content,
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
        public IActionResult GetById([FromRoute] Guid id)
        {
            var post = _dbContext.Posts.Find(id);
            var author = _dbContext.Users.Find(post?.UserId);
            var category = _dbContext.Categories.Find(post?.CategoryId);
            if (post == null || author == null || category == null)
            {
                return NotFound();
            }

            var postDto = new GetPostResDto{
                PostId = post.PostId,
                Title = post.Title,
                Content = post.Content,
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
        public IActionResult Create([FromBody] CreatePostReqDto post)
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

            return CreatedAtAction(nameof(GetById), new {id=newPostDto.PostId}, newPostDto);
        }
    
    }
}
