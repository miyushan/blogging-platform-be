using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using blogging_platform.API.Models.Domain;

namespace blogging_platform.API.Models.DTO
{
    public class SignInUserReqDto
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}