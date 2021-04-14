using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace EncryptMessage.Models
{
    public class EFMessageRepository : IMessageRepository
    {
        public IQueryable<Message> Messages => dataContext.Messages;

        private readonly DataContext dataContext;

        public EFMessageRepository(DataContext dataContext)
        {
            this.dataContext = dataContext;
        }

        public async Task AddMessageAsync(Message message)
        {
            dataContext.Messages.Add(message);
            await dataContext.SaveChangesAsync();
        }

        public async Task RemoveMessageByIdAsync(string code)
        {
            var message = await FindByCodeAsync(code);
            dataContext.Messages.Remove(message);
            await dataContext.SaveChangesAsync();
        }

        public async Task UpdateMessageAsync(Message message)
        {
            dataContext.Messages.Update(message);
            await dataContext.SaveChangesAsync();
        }

        public async Task<Message> FindByCodeAsync(string code)
        {
            return await dataContext.Messages.FirstOrDefaultAsync(m => m.Code == code);
        }
    }
}
