using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace blogging_platform.API.Models.DTO
{
    public class CategoryDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}