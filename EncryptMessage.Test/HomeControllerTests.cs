using EncryptMessage.Controllers;
using EncryptMessage.Models;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Threading.Tasks;
using Xunit;

namespace EncryptMessage.Test
{
    public class HomeControllerTests
    {
        [Fact]
        public async Task CreateMessage_ModelError()
        {
            // Arrange
            var mockEncryptor = new Mock<IMessageEncryptor>();
            var mockRepository = new Mock<IMessageRepository>();
            mockEncryptor.Setup(e => e.EncryptMessage(It.IsAny<string>(), It.IsAny<string>())).Returns(new Message());
            HomeController controller = new HomeController(mockEncryptor.Object, mockRepository.Object);
            controller.ModelState.AddModelError("Message", "Message is required");

            // Act
            var result = await controller.Create(new MessageViewModel());

            // Assert
            Assert.NotNull(result);
            mockEncryptor.Verify(e => e.EncryptMessage(It.IsAny<string>(), It.IsAny<string>()), Times.Never());
            mockRepository.Verify(r => r.AddMessageAsync(It.IsAny<Message>()), Times.Never());
        }

        [Fact]
        public async Task CreateMessage_SaveModel()
        {
            // Arrange
            var mockEncryptor = new Mock<IMessageEncryptor>();
            var mockRepository = new Mock<IMessageRepository>();

            Message message = new Message { Id = "123456" };
            MessageViewModel viewModel = new MessageViewModel { Key = "Key", Message = "Message" };

            mockEncryptor.Setup(e => e.EncryptMessage(viewModel.Message, viewModel.Key)).Returns(message);
            HomeController controller = new HomeController(mockEncryptor.Object, mockRepository.Object);

            // Act
            var result = (await controller.Create(viewModel)) as RedirectToActionResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Success", result.ActionName);
            mockEncryptor.Verify(e => e.EncryptMessage(viewModel.Message, viewModel.Key), Times.Once());
            mockRepository.Verify(r => r.AddMessageAsync(message), Times.Once());
        }

        [Fact]
        public async Task FindMessage_ExistingCode()
        {
            // Arrange
            string existingCode = "123456";
            var mockRepository = new Mock<IMessageRepository>();
            mockRepository.Setup(r => r.FindByIdAsync(existingCode)).Returns(Task.FromResult(new Message()));
            HomeController controller = new HomeController(null, mockRepository.Object);

            // Act
            var resultView = (await controller.Message(existingCode)) as ViewResult;
            var resultRedirect = (await controller.Message("654321")) as RedirectToActionResult;

            // Assert
            var vm = resultView.Model as MessageViewModel;

            Assert.NotNull(resultView);
            Assert.NotNull(resultRedirect);
            Assert.Equal(existingCode, vm.Code);
            Assert.Equal("Create", resultRedirect.ActionName);
            mockRepository.Verify(r => r.FindByIdAsync(existingCode), Times.Once());
            mockRepository.Verify(r => r.FindByIdAsync(It.IsAny<string>()), Times.Exactly(2));
        }

        [Fact]
        public async Task FindMessage_GetSecretMessage()
        {
            // Arrange
            var messageValue = "Encrypted message";
            var viewModel = new MessageViewModel
            {
                Code = "123456",
                Key = "secret"
            };
            var mockEncryptor = new Mock<IMessageEncryptor>();
            var mockRepository = new Mock<IMessageRepository>();
            mockEncryptor.Setup(e => e.DecryptMessage(It.IsAny<Message>(), viewModel.Key)).Returns(messageValue);
            mockRepository.Setup(e => e.FindByIdAsync(viewModel.Code)).Returns(Task.FromResult(new Message()));
            HomeController controller = new HomeController(mockEncryptor.Object, mockRepository.Object);

            // Act
            var result = (await controller.Message(viewModel)) as ViewResult;

            // Assert
            var vm = result.Model as MessageViewModel;

            Assert.NotNull(result);
            Assert.Equal(messageValue, vm.Message);
        }

        [Fact]
        public async Task FindMessage_ModelError()
        {
            // Arrange
            var mockEncryptor = new Mock<IMessageEncryptor>();
            var mockRepository = new Mock<IMessageRepository>();
            HomeController controller = new HomeController(mockEncryptor.Object, mockRepository.Object);
            controller.ModelState.AddModelError("Key", "Key is required");

            // Act
            var result = (await controller.Message(new MessageViewModel())) as ViewResult;

            // Assert
            Assert.NotNull(result);
            mockEncryptor.Verify(e => e.DecryptMessage(It.IsAny<Message>(), It.IsAny<string>()), Times.Never());
            mockRepository.Verify(r => r.FindByIdAsync(It.IsAny<string>()), Times.Never());
        }

        [Fact]
        public async Task FindMessage_IncorrectKey()
        {
            // Arrange
            var mockEncryptor = new Mock<IMessageEncryptor>();
            var mockRepository = new Mock<IMessageRepository>();
            mockEncryptor.Setup(e => e.DecryptMessage(It.IsAny<Message>(), It.IsAny<string>())).Returns<string>(null);
            mockRepository.Setup(e => e.FindByIdAsync(It.IsAny<string>())).Returns(Task.FromResult(new Message()));
            HomeController controller = new HomeController(mockEncryptor.Object, mockRepository.Object);

            // Act
            var result = (await controller.Message(new MessageViewModel())) as ViewResult;

            // Assert
            var vm = result.Model as MessageViewModel;
            Assert.NotNull(result);
            Assert.Null(vm?.Message);
            Assert.False(result.ViewData.ModelState.IsValid);
        }
    }
}
