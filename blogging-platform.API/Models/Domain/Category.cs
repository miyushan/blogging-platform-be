using System;

namespace blogging_platform.API.Models.Domain;

public class Category
{
    public Guid CategoryId { get; set; }
    public string Name { get; set; } = string.Empty;
}
