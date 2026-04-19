using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using BookStore.Data.Models.Identity;
using System;
using System.Threading.Tasks;

namespace BookStore.Services
{
    public static class RolesInitializer
    {
        public const string AdministratorRole = "Administrator";
        public const string UserRole = "User";

        public static async Task Initialize(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            string[] roleNames = { AdministratorRole, UserRole };

            foreach (var roleName in roleNames)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }

            var adminEmail = "admin@bookstore.com";
            var adminUser = await userManager.FindByEmailAsync(adminEmail);

            if (adminUser == null)
            {
                adminUser = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    DisplayName = "Admin",
                    Bio = "Bookstore Administrator",
                    FavoriteGenre = "Classics",
                    BirthYear = 1990,
                    RegistrationDate = DateTime.UtcNow
                };

                var createAdmin = await userManager.CreateAsync(adminUser, "AdminPassword123!");
                if (createAdmin.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, AdministratorRole);
                }
            }
        }
    }
}