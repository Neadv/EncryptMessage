using EncryptMessage.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace EncryptMessage.Test
{
    public class AesEncryptorTests
    {
        [Fact]
        public void CheckEncryptionForSymetric()
        {
            // Arrange
            AesEncryptor aes = new AesEncryptor();
            string value = "Test message";
            string key = "Test Key";

            // Act
            Message message = aes.EncryptMessage(value, key);
            string decryptValue = aes.DecryptMessage(message, key);

            // Assert
            Assert.Equal(value, decryptValue);
        }

        [Fact]
        public void Decrypt_WithIncorrectKey()
        {
            // Arrange
            AesEncryptor aes = new AesEncryptor();
            string value = "Test message";
            string key = "Test Key";

            // Act
            Message message = aes.EncryptMessage(value, key);
            string decryptValue = aes.DecryptMessage(message, "incorrect key");

            // Assert
            Assert.Null(decryptValue);
        }

        [Fact]
        public void Encrypt_WithIncorrectArguments()
        {
            // Arrange
            AesEncryptor aes = new AesEncryptor();

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => aes.EncryptMessage(null, "test"));
            Assert.Throws<ArgumentNullException>(() => aes.EncryptMessage("", "test"));
            Assert.Throws<ArgumentNullException>(() => aes.EncryptMessage("test", null));
            Assert.Throws<ArgumentNullException>(() => aes.EncryptMessage("test", ""));
        }

        [Fact]
        public void Decrypt_WithIncorrectArguments()
        {
            // Arrange
            AesEncryptor aes = new AesEncryptor();

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => aes.DecryptMessage(null, "test"));
            Assert.Throws<ArgumentNullException>(() => aes.DecryptMessage(new Message(), "test"));
            Assert.Throws<ArgumentNullException>(() => aes.DecryptMessage(new Message { Value = new byte[5] }, null));
            Assert.Throws<ArgumentNullException>(() => aes.DecryptMessage(new Message { Value = new byte[5] }, ""));
        }
    }
}
