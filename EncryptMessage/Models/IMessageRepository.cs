using System.Linq;
using System.Threading.Tasks;

namespace EncryptMessage.Models
{
    public interface IMessageRepository
    {
        IQueryable<Message> Messages { get; }

        Task AddMessageAsync(Message message);

        Task RemoveMessageByIdAsync(string id);

        Task UpdateMessageAsync(Message message);

        Task<Message> FindByIdAsync(string id);
    }
}
