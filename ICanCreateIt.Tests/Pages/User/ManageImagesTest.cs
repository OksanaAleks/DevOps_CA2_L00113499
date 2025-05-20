using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using ICanCreateIt.Data;
using ICanCreateIt.Models;
using ICanCreateIt.Pages.User;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Moq;
using Xunit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace ICanCreateIt.Tests.Pages.User
{
    public class ManageImagesModelTests
    {
        [Fact]
        public async Task OnGetAsync_ShouldLoadUserImages()
        {
            

            //  Use ServiceProvider to create DbContextOptions.  
            var serviceProvider = new ServiceCollection()
                .AddEntityFrameworkInMemoryDatabase() // Add the in-memory database provider to the service collection.
                .BuildServiceProvider();                // Build the service provider.

            // Create DbContextOptions using the service provider.  
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase("TestDB")            // Specify the name of the in-memory database. 
                .UseInternalServiceProvider(serviceProvider)  
                .Options;                                  


            // Create the ApplicationDbContext using a using block. 
            using (var context = new ApplicationDbContext(options))
            {
                // Create a mock UserManager.  We use Moq to create a mock UserManager, which allows to
                // control its behavior in our test.  
                var userManagerMock = new Mock<UserManager<ApplicationUser>>(Mock.Of<IUserStore<ApplicationUser>>(), null, null, null, null, null, null, null, null);

                
                var envMock = new Mock<IWebHostEnvironment>();

                // Create a user object.  
                var user = new ApplicationUser
                {
                    Id = "testuser",
                    UserName = "testuser",
                    Name = "Test User",
                    Gender = "Male"
                };

                // Add user and images to the in-memory database.  
                context.Users.Add(user);
                context.UserImages.AddRange(
                    new UserImage { Id = 1, UserId = "testuser", FilePath = "/images/test1.jpg", Description = "Test Image 1" },
                    new UserImage { Id = 2, UserId = "testuser", FilePath = "/images/test2.jpg", Description = "Test Image 2" },
                    new UserImage { Id = 3, UserId = "otheruser", FilePath = "/images/other.jpg", Description = "Other User Image" }
                );
                context.SaveChanges(); // Save the changes to the in-memory DB.

                // Set up UserManager to return the user. 
                userManagerMock.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                    .ReturnsAsync(user);

                // Create the PageModel, passing in the actual context
                var model = new ManageImagesModel(context, userManagerMock.Object, envMock.Object);
                model.PageContext = new PageContext(); 

               

                // Call the OnGetAsync method of the PageModel.  This is the method that is for testing.
                // It should retrieve the user's images from the database.
                await model.OnGetAsync();

                

                // Verify that the Images property is populated correctly. 
                // checking the count of the images and that the correct images are present and that the
                // incorrect image is not present.
                Assert.Equal(2, model.Images.Count);
                Assert.Contains(model.Images, i => i.Id == 1);
                Assert.Contains(model.Images, i => i.Id == 2);
                Assert.DoesNotContain(model.Images, i => i.Id == 3);
            }
        }
    }
}
