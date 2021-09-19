using Microsoft.AspNetCore.Identity;
using MyWritingPlatform.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyWritingPlatform.ViewModels
{
    public class RoleInitializer
    {
        public static async Task InitializeAsync(UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
        {
            DownloadPassword dp = new DownloadPassword();
            string adminEmail = dp.DeobfuscateAdmin()[0];
            string password = dp.DeobfuscateAdmin()[1];
            if (await roleManager.FindByNameAsync("Admin") == null)
            {
                await roleManager.CreateAsync(new IdentityRole("Admin"));
            }
            if (await roleManager.FindByNameAsync("User") == null)
            {
                await roleManager.CreateAsync(new IdentityRole("User"));
            }
            if (await userManager.FindByNameAsync(adminEmail) == null)
            {
                User admin = new User
                {
                    Avatar = "/storage/avatars/admin3191274.png",
                    FirstName = "Админ",
                    LastName = "Админ",
                    Login = "Admin",
                    Email = adminEmail,
                    UserName = adminEmail,
                    EmailConfirmed = true,
                    Year = 2000,
                    DateTimeRegister = DateTime.Now
                };
              
                var result = await userManager.CreateAsync(admin, password);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(admin, "Admin");
                }
            }
        }
    }
}
