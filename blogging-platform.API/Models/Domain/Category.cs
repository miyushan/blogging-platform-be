using System;

namespace blogging_platform.API.Models.Domain;

public class Category
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
}
