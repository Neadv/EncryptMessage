using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace EncryptMessage.Models
{
    public interface IMessageRepository
    {
        IQueryable<Message> Messages { get; }

        Task AddMessageAsync(Message message);

        Task RemoveMessageByIdAsync(string id);

        Task UpdateMessageAsync(Message message);

        Task<Message> FindByCodeAsync(string id);

        Task<IEnumerable<Message>> FindByExpressionAsync(Expression<Func<Message, bool>> expression);
    }
}
