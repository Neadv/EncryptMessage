using EncryptMessage.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace EncryptMessage
{
    public static class SeedDatabaseExtension
    {
        public static void SeedDatabase(this IApplicationBuilder app)
        {
            SeedDatabaseAsync(app).GetAwaiter().GetResult();
        }

        public static async Task SeedDatabaseAsync(IApplicationBuilder app)
        {
            using (var scope = app.ApplicationServices.CreateScope())
            {
                var services = scope.ServiceProvider;
                var configuration = services.GetRequiredService<IConfiguration>();

                await SeedIdentityAsync(services, configuration);
                await SeedMessageAsync(services, configuration);
            }
        }

        private static async Task SeedMessageAsync(IServiceProvider services, IConfiguration configuration)
        {      
            var dataContext = services.GetRequiredService<DataContext>();
            var encryptor = services.GetRequiredService<IMessageEncryptor>();
            var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();

            if (dataContext.Messages.Count() == 0)
            {
                string value = configuration["SeedData:Messages:PublicMessage:Value"];
                string key = configuration["SeedData:Messages:PublicMessage:Key"];
                string code = configuration["SeedData:Messages:PublicMessage:Code"];

                var result = encryptor.EncryptMessage(value, key);
                var publicMessage = new Message
                {
                    Code = code,
                    Value = result.Value,
                    IV = result.IV
                };

                dataContext.Messages.Add(publicMessage);

                value = configuration["SeedData:Messages:PrivateMessage:Value"];
                key = configuration["SeedData:Messages:PrivateMessage:Key"];
                code = configuration["SeedData:Messages:PrivateMessage:Code"];

                var admin = await userManager.FindByNameAsync(configuration["SeedData:Admin:Username"]);

                result = encryptor.EncryptMessage(value, key);
                var privateMessage = new Message
                {
                    Code = code,
                    Value = result.Value,
                    IV = result.IV,
                    IsPrivate = true,
                    ApplicationUser = admin
                };

                dataContext.Messages.Add(privateMessage);

                await dataContext.SaveChangesAsync();
            }
        }

        private static async Task SeedIdentityAsync(IServiceProvider services, IConfiguration configuration)
        {
            var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

            string adminRole = configuration["SeedData:Admin:Role"];
            string username = configuration["SeedData:Admin:Username"];
            string password = configuration["SeedData:Admin:Password"];

            if (!await roleManager.RoleExistsAsync(adminRole))
            {
                await roleManager.CreateAsync(new IdentityRole { Name = adminRole });
            }
            if (await userManager.FindByNameAsync(username) == null)
            {
                ApplicationUser admin = new ApplicationUser { UserName = username };
                var result = await userManager.CreateAsync(admin, password);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(admin, adminRole);
                }
            }
        }
    }
}
