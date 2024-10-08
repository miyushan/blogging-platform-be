using blogging_platform.API.Models.Domain;

namespace blogging_platform.API.Models.DTO
{
    public class CreateUserResDto
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public UserType UserType { get; set; } 
    }
}