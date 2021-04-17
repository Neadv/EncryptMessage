using EncryptMessage.Models;
using EncryptMessage.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace EncryptMessage.Test
{
    public class MessageMapperTests
    {
        [Fact]
        public async Task FromViewModel_AuthorizedUser()
        {
            // Arrange
            MessageViewModel viewModel = new MessageViewModel { IsPrivate = true };
            ApplicationUser user = new ApplicationUser { UserName = "username" };
            
            var mockStore = new Mock<IUserStore<ApplicationUser>>();
            var mockUserManager = new Mock<UserManager<ApplicationUser>>(mockStore.Object, null, null, null, null, null, null, null, null);
            mockUserManager.Setup(u => u.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);

            var mockContext = new Mock<IHttpContextAccessor>();
            mockContext.Setup(c => c.HttpContext.User.Identity.IsAuthenticated).Returns(true);
            mockContext.Setup(c => c.HttpContext.User.Identity.Name).Returns(user.UserName);

            MessageMapper mapper = new MessageMapper(mockUserManager.Object, mockContext.Object);

            // Act
            var result = await mapper.FromViewModelAsync(viewModel);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(viewModel.IsPrivate, result.IsPrivate);
            Assert.NotNull(result.Code);
        }

        [Fact]
        public async Task FromViewModel_UnauthorizedUser()
        {
            // Arrange
            MessageViewModel viewModel = new MessageViewModel { IsPrivate = true };

            var mockStore = new Mock<IUserStore<ApplicationUser>>();
            var mockUserManager = new Mock<UserManager<ApplicationUser>>(mockStore.Object, null, null, null, null, null, null, null, null);

            var mockContext = new Mock<IHttpContextAccessor>();
            mockContext.Setup(c => c.HttpContext.User.Identity.IsAuthenticated).Returns(false);


            MessageMapper mapper = new MessageMapper(mockUserManager.Object, mockContext.Object);

            // Act
            var result = await mapper.FromViewModelAsync(viewModel);

            // Assert
            Assert.NotNull(result);
            Assert.NotEqual(viewModel.IsPrivate, result.IsPrivate);
            Assert.NotNull(result.Code);
        }
    }
}
