using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace blogging_platform.API.Models.Domain
{
    public class AccessToken
    {
        public Guid AccessTokenId { get; set; }
        public Guid UserId { get; set; }
        public string Token { get; set; } = string.Empty;
        public DateTime ExpiresAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public User User { get; set; } = null!;
    }
}