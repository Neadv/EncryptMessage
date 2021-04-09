namespace EncryptMessage.Models
{
    public interface IMessageEncryptor
    {
        Message EncryptMessage(string plaintext, string key);
        string DecryptMessage(Message message, string key);
    }
}
