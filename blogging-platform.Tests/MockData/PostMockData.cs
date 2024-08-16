using blogging_platform.API.Models.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blogging_platform.Tests.MockData
{
    public class PostMockData
    {
        public static List<Post> GetPosts()
        {
            return new List<Post>
            {
                new Post
                {
                    PostId = new Guid(),
                    Title = "Title 01",
                    Content = "Content 01",
                    UserId = new Guid(),
                    CategoryId = new Guid(),
                },

                new Post
                {
                    PostId = new Guid(),
                    Title = "Title 02",
                    Content = "Content 02",
                    UserId = new Guid(),
                    CategoryId = new Guid(),
                },

                new Post
                {
                    PostId = new Guid(),
                    Title = "Title 03",
                    Content = "Content 03",
                    UserId = new Guid(),
                    CategoryId = new Guid(),
                }
            };

        }
    }
}
