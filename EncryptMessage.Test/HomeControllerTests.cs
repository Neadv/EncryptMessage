using EncryptMessage.Controllers;
using EncryptMessage.Models;
using EncryptMessage.Services;
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
            var mockMapper = new Mock<IMessageMapper>();
            HomeController controller = new HomeController(mockEncryptor.Object, mockRepository.Object, null, mockMapper.Object);
            controller.ModelState.AddModelError("Message", "Message is required");

            // Act
            var result = await controller.Create(new MessageViewModel()) as ViewResult;

            // Assert
            Assert.NotNull(result);
            mockEncryptor.Verify(e => e.EncryptMessage(It.IsAny<string>(), It.IsAny<string>()), Times.Never());
            mockRepository.Verify(r => r.AddMessageAsync(It.IsAny<Message>()), Times.Never());
            mockMapper.Verify(m => m.FromViewModelAsync(It.IsAny<MessageViewModel>()), Times.Never());
        }

        [Fact]
        public async Task CreateMessage_SaveModel()
        {
            // Arrange
            var mockEncryptor = new Mock<IMessageEncryptor>();
            var mockRepository = new Mock<IMessageRepository>();
            var mockMapper = new Mock<IMessageMapper>();

            Message message = new Message { Code = "123456" };
            MessageViewModel viewModel = new MessageViewModel { Key = "Key", Message = "Message" };

            mockMapper.Setup(m => m.FromViewModelAsync(viewModel)).Returns(Task.FromResult(message));
            mockEncryptor.Setup(e => e.EncryptMessage(viewModel.Message, viewModel.Key)).Returns(new Message());
            HomeController controller = new HomeController(mockEncryptor.Object, mockRepository.Object, null, mockMapper.Object);

            // Act
            var result = (await controller.Create(viewModel)) as RedirectToActionResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Success", result.ActionName);
            Assert.Equal(result.RouteValues["id"], message.Code);
            mockEncryptor.Verify(e => e.EncryptMessage(viewModel.Message, viewModel.Key), Times.Once());
            mockRepository.Verify(r => r.AddMessageAsync(message), Times.Once());
            mockMapper.Verify(m => m.FromViewModelAsync(viewModel), Times.Once());
        }

        [Fact]
        public async Task FindMessage_ExistingCode()
        {
            // Arrange
            string existingCode = "123456";
            var mockRepository = new Mock<IMessageRepository>();
            mockRepository.Setup(r => r.FindByCodeAsync(existingCode)).Returns(Task.FromResult(new Message()));
            HomeController controller = new HomeController(null, mockRepository.Object, null, null);

            // Act
            var resultView = (await controller.Message(existingCode)) as ViewResult;
            var resultRedirect = (await controller.Message("654321")) as RedirectToActionResult;

            // Assert
            var vm = resultView.Model as ViewMessage;

            Assert.NotNull(resultView);
            Assert.NotNull(resultRedirect);
            Assert.Equal(existingCode, vm.Code);
            Assert.Equal("Create", resultRedirect.ActionName);
            mockRepository.Verify(r => r.FindByCodeAsync(existingCode), Times.Once());
            mockRepository.Verify(r => r.FindByCodeAsync(It.IsAny<string>()), Times.Exactly(2));
        }

        [Fact]
        public async Task FindMessage_GetSecretMessage()
        {
            // Arrange
            var messageValue = "Encrypted message";
            var viewModel = new ViewMessage
            {
                Code = "123456",
                Key = "secret"
            };
            var message = new Message { Code = "123456" };

            var mockEncryptor = new Mock<IMessageEncryptor>();
            var mockRepository = new Mock<IMessageRepository>();
            var mockValidator = new Mock<IMessageValidator>();

            mockEncryptor.Setup(e => e.DecryptMessage(message, viewModel.Key)).Returns(messageValue);
            mockRepository.Setup(e => e.FindByCodeAsync(viewModel.Code)).Returns(Task.FromResult(message));
            mockValidator.Setup(v => v.ValidateAsync(message, It.IsAny<string>())).Returns(Task.FromResult(MessageValidatorResult.SucceededResult));
            HomeController controller = new HomeController(mockEncryptor.Object, mockRepository.Object, mockValidator.Object, null);

            // Act
            var result = (await controller.Message(viewModel)) as ViewResult;

            // Assert
            var vm = result.Model as ViewMessage;

            Assert.NotNull(result);
            Assert.Equal(messageValue, vm.Message);
        }

        [Fact]
        public async Task FindMessage_ModelError()
        {
            // Arrange
            var mockEncryptor = new Mock<IMessageEncryptor>();
            var mockRepository = new Mock<IMessageRepository>();
            HomeController controller = new HomeController(mockEncryptor.Object, mockRepository.Object, null, null);
            controller.ModelState.AddModelError("Key", "Key is required");

            // Act
            var result = (await controller.Message(new ViewMessage())) as ViewResult;

            // Assert
            Assert.NotNull(result);
            mockEncryptor.Verify(e => e.DecryptMessage(It.IsAny<Message>(), It.IsAny<string>()), Times.Never());
            mockRepository.Verify(r => r.FindByCodeAsync(It.IsAny<string>()), Times.Never());
        }

        [Fact]
        public async Task FindMessage_IncorrectKey()
        {
            // Arrange
            var mockEncryptor = new Mock<IMessageEncryptor>();
            var mockRepository = new Mock<IMessageRepository>();
            var mockValidator = new Mock<IMessageValidator>();
            mockEncryptor.Setup(e => e.DecryptMessage(It.IsAny<Message>(), It.IsAny<string>())).Returns<string>(null);
            mockRepository.Setup(e => e.FindByCodeAsync(It.IsAny<string>())).Returns(Task.FromResult(new Message()));
            mockValidator.Setup(v => v.ValidateAsync(It.IsAny<Message>(), It.IsAny<string>())).Returns(Task.FromResult(MessageValidatorResult.SucceededResult));
            HomeController controller = new HomeController(mockEncryptor.Object, mockRepository.Object, mockValidator.Object, null);

            // Act
            var result = (await controller.Message(new ViewMessage())) as ViewResult;

            // Assert
            var vm = result.Model as MessageViewModel;
            Assert.NotNull(result);
            Assert.Null(vm?.Message);
            Assert.False(result.ViewData.ModelState.IsValid);
        }
    }
}
