using EncryptMessage.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EncryptMessage.Services
{
    public class MessageValidator : IMessageValidator
    {
        private readonly UserManager<ApplicationUser> userManager;

        public MessageValidator(UserManager<ApplicationUser> userManager)
        {
            this.userManager = userManager;
        }

        public async Task<MessageValidatorResult> ValidateAsync(Message message, string username)
        {
            if (message.IsLockout)
                return new MessageValidatorResult { LockedOut = true, Description = "Message is locked out" };

            if (message.IsPrivate || message.IsLockout)
            {
                if (string.IsNullOrWhiteSpace(username))
                    return new MessageValidatorResult { Forbidden = true, Description = "It is private message. User must be authorized." };

                var user = await userManager.FindByNameAsync(username);
                var allowedUsers = message.AllowedUsers.Select(u => u.UserId);
                if (user != message.ApplicationUser && !allowedUsers.Contains(user.Id))
                    return new MessageValidatorResult { Forbidden = true, Description = "User must be allowed." };
            }
            return MessageValidatorResult.SucceededResult;
        }
    }
}
