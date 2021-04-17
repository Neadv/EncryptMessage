using EncryptMessage.Models;
using System.Threading.Tasks;

namespace EncryptMessage.Services
{
    public interface IMessageMapper
    {
        Task<Message> FromViewModelAsync(MessageViewModel viewModel);
    }
}
