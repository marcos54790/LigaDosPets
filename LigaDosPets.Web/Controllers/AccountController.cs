using LigaDosPets.Application.Abstractions.Services;
using LigaDosPets.Application.DTOs.Usuario;
using LigaDosPets.Infrastructure.Identity;
using LigaDosPets.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace LigaDosPets.Web.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private readonly IUsuarioService _usuarioService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public AccountController(
            IUsuarioService usuarioService,
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager)
        {
            _usuarioService = usuarioService;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        // GET: /Account/Perfil
        [HttpGet]
        public async Task<IActionResult> Perfil()
        {
            var appUser = await _userManager.GetUserAsync(User);
            if (appUser == null) return RedirectToAction("Login", "Auth");

            var perfil = await _usuarioService.GetPerfilPorApplicationUserIdAsync(appUser.Id);
            if (perfil == null) return RedirectToAction("Login", "Auth");

            var model = new UpdateUsuarioDto
            {
                Nome = perfil.Nome,
                Telefone = perfil.Telefone
            };

            ViewBag.Email = perfil.Email; // para exibir e-mail atual
            return View(model);
        }

        // POST: /Account/Perfil
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Perfil(UpdateUsuarioDto model)
        {
            if (!ModelState.IsValid) return View(model);

            var appUser = await _userManager.GetUserAsync(User);
            if (appUser == null) return RedirectToAction("Login", "Auth");

            var perfil = await _usuarioService.GetPerfilPorApplicationUserIdAsync(appUser.Id);
            if (perfil == null) return RedirectToAction("Login", "Auth");

            await _usuarioService.AtualizarPerfilAsync(perfil.Id, model);

            TempData["Sucesso"] = "Perfil atualizado com sucesso.";
            return RedirectToAction("Perfil");
        }

        // GET: /Account/ChangePassword
        [HttpGet]
        public IActionResult ChangePassword() => View();

        // POST: /Account/ChangePassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var appUser = await _userManager.GetUserAsync(User);
            if (appUser == null) return RedirectToAction("Login", "Auth");

            var result = await _userManager.ChangePasswordAsync(appUser, model.CurrentPassword, model.NewPassword);
            if (result.Succeeded)
            {
                await _signInManager.RefreshSignInAsync(appUser);
                TempData["Sucesso"] = "Senha alterada com sucesso.";
                return RedirectToAction("Perfil");
            }

            foreach (var error in result.Errors)
                ModelState.AddModelError(string.Empty, error.Description);

            return View(model);
        }

        // POST: /Account/Delete
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteAccount()
        {
            var appUser = await _userManager.GetUserAsync(User);
            if (appUser == null) return RedirectToAction("Login", "Auth");

            // Exclui o ApplicationUser (Identity)
            var result = await _userManager.DeleteAsync(appUser);
            if (!result.Succeeded)
            {
                TempData["Erro"] = "Não foi possível excluir a conta.";
                return RedirectToAction("Perfil");
            }

            // O relacionamento com Usuario (domínio) pode ser excluído em cascata se configurado, ou via repositório.
            // Vamos remover manualmente o registro de domínio se não houver cascade.
            var perfil = await _usuarioService.GetPerfilPorApplicationUserIdAsync(appUser.Id);
            if (perfil != null)
            {
                // Adicione um método DesativarUsuarioAsync ou DeleteAsync no IUsuarioService.
                // Por simplicidade, vamos desativar (soft delete).
                await _usuarioService.DesativarUsuarioAsync(perfil.Id);
            }

            await _signInManager.SignOutAsync();
            TempData["Sucesso"] = "Sua conta foi excluída permanentemente.";
            return RedirectToAction("Index", "Home");
        }
    }
}