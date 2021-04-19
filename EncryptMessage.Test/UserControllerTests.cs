using EncryptMessage.Models;
using EncryptMessage.Controllers;
using Microsoft.AspNetCore.Identity;
using Moq;
using System;
using System.Linq.Expressions;
using System.Security.Claims;
using Xunit;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace EncryptMessage.Test
{
    public class UserControllerTests
    {
        [Fact]
        public async Task Messages_GetUserMessages()
        {
            // Arrange
            Message[] messages = new[] { new Message { Code = "1" }, new Message { Code = "2" } };

            var controller = Arrange(out var mockRepository, out var mockUserManager, out var mockEncryptor);

            mockRepository.Setup(r => r.FindByExpressionAsync(It.IsAny<Expression<Func<Message, bool>>>())).ReturnsAsync(messages);
            mockUserManager.Setup(u => u.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(new ApplicationUser());

            // Act
            var result = (await controller.Messages()) as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(messages, result.Model as Message[]);
            mockUserManager.Verify(u => u.GetUserAsync(It.IsAny<ClaimsPrincipal>()), Times.Once());
            mockRepository.Verify(u => u.FindByExpressionAsync(It.IsAny<Expression<Func<Message, bool>>>()), Times.Once());
        }

        [Fact]
        public async Task RemoveMessage_ValidCode()
        {
            // Arrange
            string code = "123456";
            var user = new ApplicationUser();
            var message = new Message { Code = code, ApplicationUser = user };

            var controller = Arrange(out var mockRepository, out var mockUserManager, out var mockEncryptor);

            mockUserManager.Setup(u => u.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);
            mockRepository.Setup(r => r.FindByCodeAsync(code)).ReturnsAsync(message);

            // Act
            var result = await controller.RemoveMessage(code);

            // Assert
            Assert.NotNull(result);
            mockRepository.Verify(r => r.RemoveMessageByIdAsync(code), Times.Once());
        }

        [Fact]
        public async Task RemoveMessage_InvalidCode()
        {
            // Arrange
            string code = "123456";

            var controller = Arrange(out var mockRepository, out var mockUserManager, out var mockEncryptor);

            mockUserManager.Setup(u => u.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(new ApplicationUser());
            mockRepository.Setup(r => r.FindByCodeAsync(code)).ReturnsAsync((Message)null);

            // Act
            var result = await controller.RemoveMessage(code);

            // Assert
            Assert.NotNull(result);
            mockRepository.Verify(r => r.RemoveMessageByIdAsync(code), Times.Never());
        }

        [Fact]
        public async Task Edit_GetMessageByValidCode()
        {
            // Arrange
            string code = "123456";
            var user = new ApplicationUser();
            var message = new Message { Code = code, ApplicationUser = user, IsDisposable = true, LockoutOnFailure = true };

            var controller = Arrange(out var mockRepository, out var mockUserManager, out var mockEncryptor);

            mockUserManager.Setup(u => u.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);
            mockRepository.Setup(r => r.FindByCodeAsync(code)).ReturnsAsync(message);

            // Act
            var result = await controller.Edit(code) as ViewResult;

            // Assert
            Assert.NotNull(result);
            var model = result.Model as EditMessageViewModel;
            Assert.Equal(message.IsDisposable, model.IsDisposable);
            Assert.Equal(message.LockoutOnFailure, model.LookoutOnFailure);
        }

        [Fact]
        public async Task Edit_GetMessageByInvalidCode()
        {
            // Arrange
            string code = "123456";

            var controller = Arrange(out var mockRepository, out var mockUserManager, out var mockEncryptor);

            mockUserManager.Setup(u => u.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(new ApplicationUser());
            mockRepository.Setup(r => r.FindByCodeAsync(code)).ReturnsAsync((Message)null);

            // Act
            var result = await controller.Edit(code) as RedirectToActionResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Messages", result.ActionName);
        }

        [Fact]
        public async Task Edit_SaveMessage()
        {
            // Arrange
            string code = "123456";
            var user = new ApplicationUser();
            var message = new Message { Code = code, IsDisposable = true, ApplicationUser = user };

            var controller = Arrange(out var mockRepository, out var mockUserManager, out var mockEncryptor);

            mockUserManager.Setup(u => u.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);
            mockRepository.Setup(r => r.FindByCodeAsync(code)).ReturnsAsync(message);

            // Act
            var result = await controller.Edit(new EditMessageViewModel { Code = code, IsDisposable = false });

            // Assert
            Assert.NotNull(result);
            Assert.False(message.IsDisposable);
            mockRepository.Verify(r => r.UpdateMessageAsync(message), Times.Once());
        }

        [Fact]
        public async Task Edit_SaveMessageInvalidCode()
        {
            // Arrange
            string code = "123456";

            var controller = Arrange(out var mockRepository, out var mockUserManager, out var mockEncryptor);

            mockUserManager.Setup(u => u.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(new ApplicationUser());
            mockRepository.Setup(r => r.FindByCodeAsync(code)).ReturnsAsync((Message)null);

            // Act
            var result = await controller.Edit(new EditMessageViewModel { Code = code, IsDisposable = false });

            // Assert
            Assert.NotNull(result);
            mockRepository.Verify(r => r.UpdateMessageAsync(It.IsAny<Message>()), Times.Never());
        }

        [Fact]
        public void Unlock_ValidKey()
        {
            // Arrange & Act
            var result = ArrangeAndActUnlock(out var message, out var mockRepository, false);

            // Assert
            Assert.NotNull(result);
            Assert.False(message.IsLockout);
            mockRepository.Verify(r => r.UpdateMessageAsync(message), Times.Once());
        }

        [Fact]
        public void Unlock_InvalidCode()
        {
            // Arrange & Act
            var result = ArrangeAndActUnlock(out var message, out var mockRepository, true);

            // Assert
            Assert.NotNull(result);
            Assert.True(message.IsLockout);
            mockRepository.Verify(r => r.UpdateMessageAsync(message), Times.Never());
        }

        private IActionResult ArrangeAndActUnlock(out Message message, out Mock<IMessageRepository> mockRepository, bool returnNull)
        {
            // Arrange
            string code = "123456";
            var user = new ApplicationUser();
            message = new Message { Code = code, IsLockout = true, ApplicationUser = user };

            var controller = Arrange(out mockRepository, out var mockUserManager, out var mockEncryptor);

            mockUserManager.Setup(u => u.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);
            mockRepository.Setup(r => r.FindByCodeAsync(code)).ReturnsAsync(message);
            mockEncryptor.Setup(e => e.DecryptMessage(It.IsAny<Message>(), It.IsAny<string>())).Returns(returnNull ? null : "decrypted message");

            // Act
            return controller.Unlock(new ViewMessage { Code = code, Key = "123456" }).Result;
        }

        private UserController Arrange(out Mock<IMessageRepository> repository, out Mock<UserManager<ApplicationUser>> userManager, out Mock<IMessageEncryptor> encryptor)
        {
            var mockStore = new Mock<IUserStore<ApplicationUser>>();
            userManager = new Mock<UserManager<ApplicationUser>>(mockStore.Object, null, null, null, null, null, null, null, null);
            repository = new Mock<IMessageRepository>();
            encryptor = new Mock<IMessageEncryptor>();

            return new UserController(repository.Object, userManager.Object, encryptor.Object);
        }
    }
}
