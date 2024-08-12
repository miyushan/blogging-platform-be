using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using blogging_platform.API.Models.Domain;
using blogging_platform.API.Models.DTO;
using blogging_platform.API.Data;

namespace blogging_platform.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly BloggingPlatformDbContext dbContext;

        public CategoriesController(BloggingPlatformDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        // GET: All categories
        // GET: baseUrl/api/Categories
        [HttpGet]
        public IActionResult GetAll()
        {
            // Get data from database
            var categories = dbContext.Categories.ToList();

            // Map domain models to DTOs
            var categoryDto = new List<CategoryDto>();
            
            foreach (var category in categories)
            {   
                categoryDto.Add(new CategoryDto(){
                    Id = category.Id,
                    Name = category.Name
                });
            }

            // Return DTOs
            return Ok(categories);
        }

        // GET: Single category     
        // GET: baseUrl/api/Categories/:id
        [HttpGet]
        [Route("{id:Guid}")]
        public IActionResult GetById([FromRoute] Guid id)
        {
            // Get data from database
            var category = dbContext.Categories.Find(id);
            if (category == null)
            {
                return NotFound();
            }

            // Map domain models to DTOs
            var categoryDto = new CategoryDto{
                Id = category.Id,
                Name = category.Name
            };
            return Ok(categoryDto);
        }
    
        // POST: Add new category
        // POST: baseUrl/api/Categories
        [HttpPost]
        public IActionResult Add([FromBody] AddCategoryReqDto category)
        {
            // Map DTO to domain model
            var newCategory = new Category
            {
                Id = Guid.NewGuid(),
                Name = category.Name
            };

            // Add domain model to database
            dbContext.Categories.Add(newCategory);
            dbContext.SaveChanges();

            // Return DTO
            var newCategoryDto = new CategoryDto{
                Id = newCategory.Id,
                Name = newCategory.Name
            };

            return CreatedAtAction(nameof(GetById), new {id=newCategoryDto.Id}, newCategoryDto);
        }
    
    }
}
