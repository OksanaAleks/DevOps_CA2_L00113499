using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using ICanCreateIt.Pages.Account;
using ICanCreateIt.Models;
using Microsoft.AspNetCore.Authentication;

namespace ICanCreateIt.Tests.Pages.Account
{
    public class LogoutTest
    {
        [Fact]
        public async Task OnPost_ShouldCallSignOutAndRedirectToIndex()
        {
            // Arrange
            var userStoreMock = new Mock<IUserStore<ApplicationUser>>();
            // Create a real UserManager
            var userManager = new UserManager<ApplicationUser>(
                userStoreMock.Object,
                null, null, null, null, null, null, null, null);
            // Mock dependencies for SignInManager
            var contextAccessor = new Mock<IHttpContextAccessor>();
            var claimsFactory = new Mock<IUserClaimsPrincipalFactory<ApplicationUser>>();
            var logger = new Mock<ILogger<SignInManager<ApplicationUser>>>();
            var schemes = new Mock<IAuthenticationSchemeProvider>();
            var confirmation = new Mock<IUserConfirmation<ApplicationUser>>();

            // Create a mock SignInManager with all dependencies
            var signInManagerMock = new Mock<SignInManager<ApplicationUser>>(
                userManager,
                contextAccessor.Object,
                claimsFactory.Object,
                null, logger.Object, schemes.Object, confirmation.Object
            );

            signInManagerMock.Setup(s => s.SignOutAsync())
                             .Returns(Task.CompletedTask)
                             .Verifiable();

            var model = new LogoutModel(signInManagerMock.Object);

            var httpContext = new DefaultHttpContext();
            var tempData = new TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>());
            model.TempData = tempData;

            
            var result = await model.OnPost();

            
            var redirect = Assert.IsType<RedirectToPageResult>(result);
            Assert.Equal("/Index", redirect.PageName);
            Assert.Equal("You have been successfully logged out.", model.TempData["LogoutMessage"]);
            signInManagerMock.Verify(s => s.SignOutAsync(), Times.Once);
        }
    }
}



