using EncryptMessage.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EncryptMessage.Components
{
    public class LoginWidgetViewComponent : ViewComponent
    {
        private readonly UserManager<ApplicationUser> userManager;

        public LoginWidgetViewComponent(UserManager<ApplicationUser> userManager)
        {
            this.userManager = userManager;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            string username = null;
            if (User.Identity.IsAuthenticated)
            {
                var user = await userManager.FindByNameAsync(User.Identity.Name);
                if (await userManager.IsInRoleAsync(user, "Admin"))
                {
                    ViewBag.IsAdmin = true;
                }
                return View(model: user.UserName);
            }
            return View(model: username);
        }
    }
}
