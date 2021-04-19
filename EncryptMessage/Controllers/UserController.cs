using EncryptMessage.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace EncryptMessage.Controllers
{
    [Authorize]
    public class UserController : Controller
    {
        private readonly IMessageRepository repository;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IMessageEncryptor encryptor;

        public UserController(IMessageRepository repository, UserManager<ApplicationUser> userManager, IMessageEncryptor encryptor)
        {
            this.repository = repository;
            this.userManager = userManager;
            this.encryptor = encryptor;
        }

        public async Task<IActionResult> Messages()
        {
            var user = await userManager.GetUserAsync(User);
            var messages = await repository.FindByExpressionAsync(m => m.ApplicationUserId == user.Id);
            return View(messages);
        }

        [HttpPost]
        public async Task<IActionResult> RemoveMessage(string code)
        {
            if (!string.IsNullOrWhiteSpace(code))
            {
                var message = await repository.FindByCodeAsync(code);
                var user = await userManager.GetUserAsync(User);
                if (message != null && user == message.ApplicationUser)
                {
                    await repository.RemoveMessageByIdAsync(code);
                }
            }
            return RedirectToAction(nameof(Messages));
        }

        public async Task<IActionResult> Edit(string id)
        {
            var message = await repository.FindByCodeAsync(id);
            var user = await userManager.GetUserAsync(User);
            if (message != null && user == message.ApplicationUser)
            {
                var viewModel = new EditMessageViewModel
                {
                    Code = id,
                    IsDisposable = message.IsDisposable,
                    LookoutOnFailure = message.LockoutOnFailure,
                    IsPrivate = message.IsPrivate
                };
                return View(viewModel);
            }
            return RedirectToAction(nameof(Messages));
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EditMessageViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var message = await repository.FindByCodeAsync(viewModel.Code);
                var user = await userManager.GetUserAsync(User);
                if (message != null && !message.IsLockout && user == message.ApplicationUser)
                {
                    message.IsPrivate = viewModel.IsPrivate;
                    message.IsDisposable = viewModel.IsDisposable;
                    message.LockoutOnFailure = viewModel.LookoutOnFailure;
                    await repository.UpdateMessageAsync(message);
                }
            }
            return RedirectToAction(nameof(Messages));
        }

        public IActionResult Unlock(string id)
        {
            return View(new ViewMessage { Code = id });
        }

        [HttpPost]
        public async Task<IActionResult> Unlock(ViewMessage viewMessage)
        {
            if (ModelState.IsValid)
            {
                var message = await repository.FindByCodeAsync(viewMessage.Code);
                if (message != null)
                {
                    var messageValue = encryptor.DecryptMessage(message, viewMessage.Key);
                    var user = await userManager.GetUserAsync(User);
                    if (messageValue != null && user == message.ApplicationUser && message.IsLockout)
                    {
                        message.IsLockout = false;
                        await repository.UpdateMessageAsync(message);
                    }
                }
            }
            return RedirectToAction(nameof(Messages));
        }
    }
}
