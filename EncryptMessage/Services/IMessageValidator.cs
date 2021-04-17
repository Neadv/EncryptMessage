using EncryptMessage.Models;
using System.Threading.Tasks;

namespace EncryptMessage.Services
{
    public interface IMessageValidator
    {
        Task<MessageValidatorResult> ValidateAsync(Message message, string username);
    }

    public class MessageValidatorResult
    {
        public bool Succeeded { get; set; } = false;
        public bool LockedOut { get; set; } = false;
        public bool Forbidden { get; set; } = false;
        public string Description { get; set; } = null;

        public static MessageValidatorResult SucceededResult => new MessageValidatorResult { Succeeded = true };
    }
}
