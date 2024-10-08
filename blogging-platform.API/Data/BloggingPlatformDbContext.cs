using System;
using blogging_platform.API.Models.Domain;
using Microsoft.EntityFrameworkCore;

namespace blogging_platform.API.Data;

public class BloggingPlatformDbContext: DbContext
{

    public BloggingPlatformDbContext(DbContextOptions dbContextOptions): base(dbContextOptions)
    {
        
    }

    public DbSet<User> Users { get; set; }

    public DbSet<Post> Posts { get; set; }

    public DbSet<Category> Categories { get; set; }

    public DbSet<Comment> Comments { get; set; }

    public DbSet<AccessToken> AccessTokens { get; set; }

    public DbSet<RefreshToken> RefreshTokens { get; set; }
}
