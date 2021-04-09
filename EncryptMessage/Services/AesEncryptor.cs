using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace EncryptMessage.Models
{
    public class AesEncryptor : IMessageEncryptor
    {
        public Message EncryptMessage(string plaintext, string key)
        {
            if (plaintext == null || plaintext.Length == 0)
                throw new ArgumentNullException(nameof(plaintext));
            if (key == null || key.Length == 0)
                throw new ArgumentNullException(nameof(key));

            Message message = new Message();
            using (Aes aesAlg = Aes.Create())
            {
                using (SHA256 sha256 = SHA256.Create())
                {
                    aesAlg.GenerateIV();
                    aesAlg.Key = sha256.ComputeHash(Encoding.UTF8.GetBytes(key));

                    ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);
                    byte[] encrypted;

                    using (MemoryStream msEncrypt = new MemoryStream())
                    {
                        using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                        {
                            using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                            {
                                //Write all data to the stream.
                                swEncrypt.Write(plaintext);
                            }
                            encrypted = msEncrypt.ToArray();
                        }
                    }


                    message.Value = encrypted;
                    message.IV = aesAlg.IV;
                }
            }

            return message;
        }

        public string DecryptMessage(Message message, string key)
        {
            if (message.Value == null || message.Value.Length == 0)
                throw new ArgumentNullException(nameof(message.Value));
            if (message.IV == null || message.IV.Length == 0)
                throw new ArgumentNullException(nameof(message.IV));
            if (key == null || key.Length == 0)
                throw new ArgumentNullException(nameof(key));

            string plaintext = null;

            using (Aes aesAlg = Aes.Create())
            {
                using (SHA256 sha256 = SHA256.Create())
                {
                    aesAlg.Key = sha256.ComputeHash(Encoding.UTF8.GetBytes(key));
                    aesAlg.IV = message.IV;
                    ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                    using (MemoryStream msDecrypt = new MemoryStream(message.Value))
                    {
                        using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                        {
                            using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                            {

                                plaintext = srDecrypt.ReadToEnd();
                            }
                        }
                    }
                }
            }

            return plaintext;
        }
    }
}
