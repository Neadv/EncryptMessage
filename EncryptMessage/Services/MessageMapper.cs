using EncryptMessage.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
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

                    message.AllowedUsers = new List<AllowedUserMessages>();
                    foreach (var username in viewModel.Users)
                    {
                        var user = await userManager.FindByNameAsync(username);
                        message.AllowedUsers.Add(new AllowedUserMessages { Message = message, User = user });
                    }
                }
            }

            RandomString randomString = new RandomString(6);
            message.Code = randomString.Next();

            return message;
        }
    }
}
