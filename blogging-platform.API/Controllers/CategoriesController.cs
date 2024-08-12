using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using blogging_platform.API.Models.Domain;
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
        // GET: api/Categories
        [HttpGet]
        public IActionResult GetAll()
        {
            var categories = dbContext.Categories.ToList();

            return Ok(categories);
        }

        // GET: Single category     
        // GET: api/Categories/:id
        [HttpGet]
        [Route("{id:Guid}")]
        public IActionResult GetById([FromRoute] Guid id)
        {
            var category = dbContext.Categories.Find(id);
            if (category == null)
            {
                return NotFound();
            }

            return Ok(category);
        }
    }
}
