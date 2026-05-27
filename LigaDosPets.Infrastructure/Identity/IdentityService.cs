using LigaDosPets.Application.Abstractions.Identity;
using Microsoft.AspNetCore.Identity;

namespace LigaDosPets.Infrastructure.Identity
{
    public class IdentityService : IIdentityService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public IdentityService(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<string> CreateUserAsync(string email, string password, string role)
        {
            var user = new ApplicationUser
            {
                UserName = email,
                Email = email,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(user, password);
            if (!result.Succeeded)
                throw new InvalidOperationException(string.Join(", ", result.Errors.Select(e => e.Description)));

            await AddToRoleAsync(user.Id, role);
            return user.Id;
        }

        public async Task AddToRoleAsync(string userId, string role)
        {
            if (!await _roleManager.RoleExistsAsync(role))
                await _roleManager.CreateAsync(new IdentityRole(role));

            var user = await _userManager.FindByIdAsync(userId);
            if (user != null)
                await _userManager.AddToRoleAsync(user, role);
        }
        public async Task AlterarRoleAsync(string applicationUserId, string novaRole)
        {
            var user = await _userManager.FindByIdAsync(applicationUserId);
            if (user == null) throw new InvalidOperationException("Usuário Identity não encontrado.");

            var rolesAtuais = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, rolesAtuais);
            await _userManager.AddToRoleAsync(user, novaRole);
        }
    }
}