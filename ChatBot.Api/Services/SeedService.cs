using ChatBot.Api.Data;
using ChatBot.Api.Models.Users;
using Microsoft.AspNetCore.Identity;

namespace ChatBot.Api.Services
{
    public class SeedService : ISeedService
    {
        private readonly UserManager<User> userManager;
        private readonly RoleManager<Role> roleManager;
        private readonly ChatBotDbContext context;

        private const string UserRole = "User";
        private const string AdminRole = "Admin";

        public SeedService(UserManager<User> userManager, RoleManager<Role> roleManager, ChatBotDbContext context)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.context = context;
        }

        public void Seed()
        {
            CreateRole(UserRole).GetAwaiter().GetResult();
            CreateRole(AdminRole).GetAwaiter().GetResult();
            CreateUser("admin@chatbot.com", "Admin", "Test@2025!", roles: new List<string> { UserRole, AdminRole }).GetAwaiter().GetResult();
        }

        private async Task<IdentityResult> CreateRole(string roleName)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                var role = new Role
                {
                    Name = roleName
                };
                return await roleManager.CreateAsync(role);
            }
            return default;
        }

        private async Task<IdentityResult> CreateUser(string email, string name, string password, IEnumerable<string> roles)
        {
            var request = await userManager.FindByEmailAsync(email);
            if (request == null)
            {
                var user = new User
                {
                    UserName = email,
                    Email = email,
                    EmailConfirmed = true,
                };

                IdentityResult result = await userManager.CreateAsync(user, password);

                if (result.Succeeded)
                {
                    await userManager.AddToRolesAsync(user, roles);
                }

                return result;
            }
            else
            {
                return default;
            }
        }
    }
}
