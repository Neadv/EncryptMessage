using EncryptMessage.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using System.Threading.Tasks;

namespace EncryptMessage.Services
{
    public class MessageMapper : IMessageMapper
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly ClaimsPrincipal user;

        public MessageMapper(UserManager<ApplicationUser> userManager, IHttpContextAccessor accessor)
        {
            this.userManager = userManager;
            this.user = accessor.HttpContext.User;
        }

        public async Task<Message> FromViewModelAsync(MessageViewModel viewModel)
        {
            Message message = new Message();
            if (user.Identity.IsAuthenticated)
            {
                var authorizedUser = await userManager.GetUserAsync(user);
                if (authorizedUser != null)
                {
                    message.ApplicationUser = authorizedUser;
                    message.IsDisposable = viewModel.IsDisposable;
                    message.IsPrivate = viewModel.IsPrivate;
                    message.LockoutOnFailure = viewModel.lockoutOnFailure;
                }
            }

            RandomString randomString = new RandomString(6);
            message.Code = randomString.Next();

            return message;
        }
    }
}
