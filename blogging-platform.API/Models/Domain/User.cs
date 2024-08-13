using System;
using System.Runtime.Serialization;

namespace blogging_platform.API.Models.Domain;


public enum UserType
{
    Admin = 1,
    Author = 2,
    Visitor = 3
}

public class User
{
    public Guid UserId { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public UserType UserType { get; set; }
    public string AccessToken { get; set; } = string.Empty;

}
