using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace blogging_platform.API.Models.DTO
{
    public class PostDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public Guid UserId { get; set; }
        public Guid CategoryId { get; set; }
    }
}