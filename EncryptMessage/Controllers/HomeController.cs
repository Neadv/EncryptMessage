using EncryptMessage.Models;
using EncryptMessage.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace EncryptMessage.Controllers
{
    public class HomeController : Controller
    {
        private readonly IMessageEncryptor encryptor;
        private readonly IMessageRepository repository;
        private readonly IMessageValidator validator;
        private readonly IMessageMapper mapper;

        public HomeController(IMessageEncryptor encryptor, IMessageRepository repository,
            IMessageValidator validator, IMessageMapper mapper)
        {
            this.encryptor = encryptor;
            this.repository = repository;
            this.validator = validator;
            this.mapper = mapper;
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(MessageViewModel messageViewModel)
        {
            if (ModelState.IsValid)
            {
                var result = await validator.ValidateCreationAsync(messageViewModel);
                if (result.Succeeded)
                {
                    var message = await mapper.FromViewModelAsync(messageViewModel);
                    var encrypt = encryptor.EncryptMessage(messageViewModel.Message, messageViewModel.Key);
                    message.Value = encrypt.Value;
                    message.IV = encrypt.IV;

                    await repository.AddMessageAsync(message);

                    return RedirectToAction(nameof(Success), new { Id = message.Code });
                }
                ModelState.AddModelError("", result.Description);
            }
            return View();
        }

        public async Task<IActionResult> Message(string id)
        {
            return await repository.FindByCodeAsync(id) == null
                   ? RedirectToAction(nameof(Create))
                   : View(new ViewMessage { Code = id });
        }

        [HttpPost]
        public async Task<IActionResult> Message(ViewMessage viewModel)
        {
            if (ModelState.IsValid)
            {
                var message = await repository.FindByCodeAsync(viewModel.Code);
                if (message != null)
                {
                    var result = await validator.ValidateAsync(message, User?.Identity.Name);
                    if (result.Succeeded)
                    {
                        string messageValue = encryptor.DecryptMessage(message, viewModel.Key);
                        if (messageValue != null)
                        {
                            if (message.IsDisposable)
                                await repository.RemoveMessageByIdAsync(message.Code);

                            return View(new ViewMessage { Code = viewModel.Code, Message = messageValue });
                        }
                        if (message.LockoutOnFailure)
                        {
                            message.IsLockout = true;
                            await repository.UpdateMessageAsync(message);
                        }
                        ModelState.AddModelError("Key", "The entered key value is incorrect ");
                    }
                    else
                    {
                        ModelState.AddModelError("", result.Description);
                    }
                }
            }
            return View();
        }

        public IActionResult Success(string id)
        {
            return View(model: id);
        }
    }
}
