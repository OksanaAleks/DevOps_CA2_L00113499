using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;
using ICanCreateIt.Models;
using ICanCreateIt.Pages.Account;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;


namespace ICanCreateIt.Tests.Pages.Account
{
    public class LoginTest
    {
        [Fact]
        public async Task OnPostAsync_ValidLogin_RedirectsToIndex()
        {
            // Arrange
            var userManager = MockUserManager<ApplicationUser>();
            var signInManager = MockSignInManager<ApplicationUser>();

            // Simulate successful login
            signInManager.Setup(s => s.PasswordSignInAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                false,
                false
            )).ReturnsAsync(SignInResult.Success);

            var model = new LoginModel(signInManager.Object)
            {
                Input = new LoginModel.InputModel
                {
                    Email = "test@example.com",
                    Password = "Password123"
                }
            };

            // Act
            var result = await model.OnPostAsync();

            // Assert
            var redirect = Assert.IsType<RedirectToPageResult>(result);
            Assert.Equal("/Account/UserHome", redirect.PageName);

        }

        private static Mock<UserManager<TUser>> MockUserManager<TUser>() where TUser : class
        {
            var store = new Mock<IUserStore<TUser>>();
            return new Mock<UserManager<TUser>>(
                store.Object, null, null, null, null, null, null, null, null
            );
        }

        private static Mock<SignInManager<TUser>> MockSignInManager<TUser>() where TUser : class
        {
            var userManager = MockUserManager<TUser>();
            var contextAccessor = new Mock<Microsoft.AspNetCore.Http.IHttpContextAccessor>();
            var claimsFactory = new Mock<IUserClaimsPrincipalFactory<TUser>>();

            return new Mock<SignInManager<TUser>>(
                userManager.Object,
                contextAccessor.Object,
                claimsFactory.Object,
                null, null, null, null
            );
        }
    }
}
