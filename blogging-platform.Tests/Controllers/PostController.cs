using blogging_platform.API.Data;
using blogging_platform.API.Controllers;
using blogging_platform.Tests.MockData;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blogging_platform.Tests.Controllers
{
    public class PostController
    {

        [Fact]
        public async Task GetAllPostsAsync_ShouldReturn200Status()
        {
            //Arrange
            var dbContext = new Mock<BloggingPlatformDbContext>();
            dbContext.Setup(_ => _.GetAllPostsAsync()).ReturnsAsync(PostMockData.GetPosts());

        }
    }
}

   
