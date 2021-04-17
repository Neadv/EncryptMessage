using EncryptMessage.Models;
using EncryptMessage.Services;
using Microsoft.AspNetCore.Identity;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace EncryptMessage.Test
{
    public class MessageValidatorTests
    {
        [Fact]
        public async Task ValidateMessage_PublicMessage()
        {
            // Arrange
            Message message = new Message { IsPrivate = false };
            MessageValidator validator = new MessageValidator(null);

            // Act
            var result = await validator.ValidateAsync(message, null);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Succeeded);
        }

        [Fact]
        public async Task ValidateMessage_UnauthorizedUser_PrivateMessage()
        {
            // Arrange
            Message message = new Message { IsPrivate = true };
            MessageValidator validator = new MessageValidator(null);

            // Act
            var result = await validator.ValidateAsync(message, null);

            // Assert
            Assert.NotNull(result);
            Assert.False(result.Succeeded);
            Assert.True(result.Forbidden);
        }

        [Fact]
        public async Task ValidateMessage_LockoutMessage()
        {
            // Arrange
            Message message = new Message { IsLockout = true };
            MessageValidator validator = new MessageValidator(null);

            // Act
            var result = await validator.ValidateAsync(message, null);

            // Assert
            Assert.NotNull(result);
            Assert.False(result.Succeeded);
            Assert.True(result.LockedOut);
        }

        [Fact]
        public async Task ValidateMessage_NotAllowedUser()
        {
            // Arrange
            string username = "username";
            Message message = new Message { IsPrivate = true, AllowedUsers = new List<AllowedUserMessages>()};
            var mockStore = new Mock<IUserStore<ApplicationUser>>();
            var mockUserManager = new Mock<UserManager<ApplicationUser>>(mockStore.Object, null, null, null, null, null, null, null, null);
            mockUserManager.Setup(u => u.FindByNameAsync(username)).ReturnsAsync(new ApplicationUser { UserName = username});
            
            MessageValidator validator = new MessageValidator(mockUserManager.Object);

            // Act
            var result = await validator.ValidateAsync(message, username);

            // Assert
            Assert.NotNull(result);
            Assert.False(result.Succeeded);
            Assert.True(result.Forbidden);
        }

        [Fact]
        public async Task ValidateMessage_AllowedUser()
        {
            // Arrange
            string username = "username";
            ApplicationUser user = new ApplicationUser { UserName = username, Id = "id" };
            Message message = new Message { IsPrivate = true, AllowedUsers = new List<AllowedUserMessages>() { new AllowedUserMessages { User = user, UserId = user.Id} } };
            var mockStore = new Mock<IUserStore<ApplicationUser>>();
            var mockUserManager = new Mock<UserManager<ApplicationUser>>(mockStore.Object, null, null, null, null, null, null, null, null);
            mockUserManager.Setup(u => u.FindByNameAsync(username)).ReturnsAsync(user);

            MessageValidator validator = new MessageValidator(mockUserManager.Object);

            // Act
            var result = await validator.ValidateAsync(message, username);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Succeeded);
        }

        [Fact]
        public async Task ValidateMessage_OwnerUser()
        {
            // Arrange
            ApplicationUser user = new ApplicationUser { UserName = "username"};
            Message message = new Message { IsPrivate = true, ApplicationUser = user, AllowedUsers = new List<AllowedUserMessages>() };
            var mockStore = new Mock<IUserStore<ApplicationUser>>();
            var mockUserManager = new Mock<UserManager<ApplicationUser>>(mockStore.Object, null, null, null, null, null, null, null, null);
            mockUserManager.Setup(u => u.FindByNameAsync(user.UserName)).ReturnsAsync(user);

            MessageValidator validator = new MessageValidator(mockUserManager.Object);

            // Act
            var result = await validator.ValidateAsync(message, user.UserName);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Succeeded);
        }
    }
}
