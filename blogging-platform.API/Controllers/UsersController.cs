using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using blogging_platform.API.Models.Domain;
using blogging_platform.API.Models.DTO;
using blogging_platform.API.Data;
using blogging_platform.API.Validations;

namespace blogging_platform.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly BloggingPlatformDbContext dbContext;

        public UsersController(BloggingPlatformDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
    
        // POST: Add new user
        // POST: baseUrl/api/Users
        [HttpPost]
        public IActionResult Add(AddUserReqDto user)
        {
            var validator = new AddUserReqValidator();
            var results = validator.Validate(user);

            if (!results.IsValid)
            {
                return BadRequest(results.Errors);
            }


            // Map DTO to domain model
            var newUser = new User
            {
                Id = Guid.NewGuid(),
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Password = user.Password,
                UserType = (UserType)Enum.Parse(typeof(UserType), user.UserType) 
            };

            // Add domain model to database
            dbContext.Users.Add(newUser);
            dbContext.SaveChanges();

            // Return DTO
            var newUserDto = new UserDto{
                Id = newUser.Id,
                FirstName = newUser.FirstName,
                LastName = newUser.LastName,
                Email = newUser.Email,
                Password = newUser.Password,
                UserType = newUser.UserType
            };

            return Ok(newUserDto);
        }
    
    }
}
