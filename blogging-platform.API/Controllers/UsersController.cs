using Microsoft.AspNetCore.Mvc;
using blogging_platform.API.Models.Domain;
using blogging_platform.API.Models.DTO;
using blogging_platform.API.Data;
using blogging_platform.API.Validations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using FluentValidation;

namespace blogging_platform.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly BloggingPlatformDbContext _dbContext;
        IConfiguration _configuration;

        public UsersController(BloggingPlatformDbContext dbContext, IConfiguration configuration)
        {
            _dbContext = dbContext;
            _configuration = configuration;
        }
    
        [Route("sign-up")]
        [HttpPost]
        public IActionResult CreateNewUser(CreateUserReqDto user)
        {
            // Validate user
            var validator = new SignUpUserReqValidator();
            var results = validator.Validate(user);
            if (!results.IsValid)
            {
                return BadRequest(results.Errors);
            }

            // Create new user
            var newUser = new User
            {
                UserId = Guid.NewGuid(),
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Password = user.Password,
                UserType = (UserType)Enum.Parse(typeof(UserType), user.UserType) 
            };
            _dbContext.Users.Add(newUser);
            _dbContext.SaveChanges();

            // Return response with DTOs
            var newUserDto = new SignUpUserResDto{
                Id = newUser.UserId,
                FirstName = newUser.FirstName,
                LastName = newUser.LastName,
                Email = newUser.Email,
                Password = newUser.Password,
                UserType = newUser.UserType
            };

            return Ok(newUserDto);
        }

        [Route("sign-in")]
        [HttpPost]
        public async Task<IActionResult> UserSignin(SignInUserReqDto user)
        {
            // Validate user
            var validator = new SignInUserReqValidator();
            var results = validator.Validate(user);
            if (!results.IsValid)
            {
                return BadRequest(results.Errors);
            }

            var existingUser = await _dbContext.Users
                .Where(e => e.Email == user.Email && e.Password == user.Password)
                .FirstOrDefaultAsync();

            if (existingUser == null)
            {
                return BadRequest("Invalid credentials");
            }

            var accessToken = await _dbContext.AccessTokens
                .Where(t => t.UserId == existingUser.UserId)
                .OrderByDescending(t => t.ExpiresAt)
                .FirstOrDefaultAsync();

            var refreshToken = await _dbContext.RefreshTokens
                .Where(t => t.UserId == existingUser.UserId)
                .OrderByDescending(t => t.ExpiresAt)
                .FirstOrDefaultAsync();

            if (accessToken == null)
            {
                accessToken = new AccessToken
                {
                    AccessTokenId = Guid.NewGuid(),
                    UserId = existingUser.UserId
                };
                _dbContext.AccessTokens.Add(accessToken);
            }

            if (refreshToken == null)
            {
                refreshToken = new RefreshToken
                {
                    RefreshTokenId = Guid.NewGuid(),
                    UserId = existingUser.UserId
                };
                _dbContext.RefreshTokens.Add(refreshToken);
            }

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, _configuration["Jwt:Subject"]),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()),
                new Claim("UserId", existingUser.UserId.ToString()),
                new Claim("FirstName", existingUser.FirstName),
                new Claim("LastName", existingUser.LastName),
                new Claim("Email", existingUser.Email)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                _configuration["Jwt:Issuer"],
                _configuration["Jwt:Audience"],
                claims,
                expires: DateTime.UtcNow.AddMinutes(10),
                signingCredentials: signIn);

            accessToken.Token = new JwtSecurityTokenHandler().WriteToken(token);
            accessToken.ExpiresAt = DateTime.UtcNow.AddMinutes(10);
            refreshToken.Token = Guid.NewGuid().ToString();
            refreshToken.ExpiresAt = DateTime.UtcNow.AddMinutes(60);

            await _dbContext.SaveChangesAsync();

            // Return response with DTOs
            var newUserDto = new SignInUserResDto
            {
                Id = existingUser.UserId,
                FirstName = existingUser.FirstName,
                LastName = existingUser.LastName,
                Email = existingUser.Email,
                Password = existingUser.Password,
                UserType = existingUser.UserType,
                AccessToken = accessToken.Token,
                RefreshToken = refreshToken.Token
            };

            return Ok(newUserDto);
        }

        [Route("sign-out")]
        [HttpPost]
        public async Task<IActionResult> UserSignOut([FromHeader(Name = "user-id") ] string userId)
        {
            if(userId == null){
                return Unauthorized();
            }

            var accessToken = await _dbContext.AccessTokens
                .Where(t => t.UserId == Guid.Parse(userId))
                .OrderByDescending(t => t.ExpiresAt)
                .FirstOrDefaultAsync();

            if (accessToken != null)
            {
                _dbContext.AccessTokens.Remove(accessToken);
            }

            var refreshToken = await _dbContext.RefreshTokens
                .Where(t => t.UserId == Guid.Parse(userId))
                .OrderByDescending(t => t.ExpiresAt)
                .FirstOrDefaultAsync();

            if (refreshToken != null)
            {
                _dbContext.RefreshTokens.Remove(refreshToken);
            }

            await _dbContext.SaveChangesAsync();

            return Ok();
        }


        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Route("get-refreshToken")]
        [HttpPost]
        public async Task<IActionResult> GetRefreshToken(string refreshToken)
        {
            var existingRefreshToken = await _dbContext.RefreshTokens
                .Where(e => e.Token == refreshToken)
                .FirstOrDefaultAsync();

            if (existingRefreshToken == null || existingRefreshToken.ExpiresAt < DateTime.UtcNow)
            {
                return BadRequest("Invalid refresh token");
            }

            var user = await _dbContext.Users
                .Where(e => e.UserId == existingRefreshToken.UserId)
                .FirstOrDefaultAsync();

            var claims = new[] {
                new Claim(JwtRegisteredClaimNames.Sub, _configuration["Jwt:Subject"]),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()),
                new Claim("UserId", user.UserId.ToString()),
                new Claim("FirstName", user.FirstName),
                new Claim("LastName", user.LastName),
                new Claim("Email", user.Email)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                _configuration["Jwt:Issuer"],
                _configuration["Jwt:Audience"],
                claims,
                expires: DateTime.UtcNow.AddMinutes(10),
                signingCredentials: signIn);

            var newAccessToken = new AccessToken
            {
                AccessTokenId = Guid.NewGuid(),
                UserId = user.UserId,
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                ExpiresAt = DateTime.UtcNow.AddMinutes(10),
            };

            _dbContext.AccessTokens.Add(newAccessToken);
            await _dbContext.SaveChangesAsync();

            return Ok(new { token = newAccessToken.Token });
        }
    }
}
