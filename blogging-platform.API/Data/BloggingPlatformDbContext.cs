using System;
using blogging_platform.API.Models.Domain;
using Microsoft.EntityFrameworkCore;

namespace blogging_platform.API.Data;

public class BloggingPlatformDbContext: DbContext
{

    public BloggingPlatformDbContext(DbContextOptions dbContextOptions): base(dbContextOptions)
    {
        
    }

    public DbSet<Post> Posts { get; set; }

    public DbSet<Category> Categories { get; set; }

    public DbSet<Comment> Comments { get; set; }
}
