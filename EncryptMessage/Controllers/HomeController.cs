using EncryptMessage.Models;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace EncryptMessage.Controllers
{
    public class HomeController : Controller
    {
        private readonly IMessageEncryptor encryptor;
        private readonly IMessageRepository repository;

        public HomeController(IMessageEncryptor encryptor, IMessageRepository repository)
        {
            this.encryptor = encryptor;
            this.repository = repository;
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
                var message = encryptor.EncryptMessage(messageViewModel.Message, messageViewModel.Key);

                RandomString randomString = new RandomString(6);
                string id = randomString.Next();

                message.Code = id;
                await repository.AddMessageAsync(message);

                return RedirectToAction(nameof(Success), new { Id = id });
            }
            return View();
        }

        public async Task<IActionResult> Message(string id)
        {
            return await repository.FindByCodeAsync(id) == null
                   ? RedirectToAction(nameof(Create))
                   : View(new MessageViewModel { Code = id });
        }

        [HttpPost]
        public async Task<IActionResult> Message(MessageViewModel viewModel)
        {
            ModelState.Remove("Message");
            if (ModelState.IsValid)
            {
                var message = await repository.FindByCodeAsync(viewModel.Code);
                if (message != null)
                {
                    string messageValue = encryptor.DecryptMessage(message, viewModel.Key);
                    if (messageValue != null)
                    {
                        return View(new MessageViewModel { Code = viewModel.Code, Message = messageValue });
                    }
                    ModelState.AddModelError("Key", "The entered key value is incorrect ");
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
