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
    }
}
