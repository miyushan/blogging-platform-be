using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using blogging_platform.API.Models.Domain;


namespace blogging_platform.API.Models.DTO
{
    public class SignInUserResDto
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public UserType UserType { get; set; } 
        public string AccessToken { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;

    }
}