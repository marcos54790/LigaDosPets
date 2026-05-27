using LigaDosPets.Application.Abstractions.Services;
using LigaDosPets.Application.DTOs.Usuario;
using LigaDosPets.Infrastructure.Identity;
using LigaDosPets.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LigaDosPets.Web.Controllers
{
    [Authorize]
    public class UsuarioController : Controller
    {
        private readonly IUsuarioService _usuarioService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public UsuarioController(
            IUsuarioService usuarioService,
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager)
        {
            _usuarioService = usuarioService;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        // ── Perfil (GET) ────────────────────────────────────────
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

            ViewBag.Email = perfil.Email;
            return View(model);
        }

        // ── Perfil (POST) ───────────────────────────────────────
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

        // ── Alterar Senha (GET) ─────────────────────────────────
        [HttpGet]
        public IActionResult ChangePassword() => View();

        // ── Alterar Senha (POST) ────────────────────────────────
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

        // ── Excluir Conta (POST) ────────────────────────────────
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteAccount()
        {
            var appUser = await _userManager.GetUserAsync(User);
            if (appUser == null) return RedirectToAction("Login", "Auth");

            var perfil = await _usuarioService.GetPerfilPorApplicationUserIdAsync(appUser.Id);

            // Remove o registro do domínio (se existir)
            if (perfil != null)
                await _usuarioService.DesativarUsuarioAsync(perfil.Id);

            // Remove o usuário do Identity
            var result = await _userManager.DeleteAsync(appUser);
            if (!result.Succeeded)
            {
                TempData["Erro"] = "Não foi possível excluir a conta.";
                return RedirectToAction("Perfil");
            }

            await _signInManager.SignOutAsync();
            TempData["Sucesso"] = "Sua conta foi excluída permanentemente.";
            return RedirectToAction("Index", "Home");
        }

        // ── Completar Perfil (GET) ──────────────────────────────
        [HttpGet]
        public IActionResult CompletarPerfil()
        {
            return View("~/Views/Dashboard/Form.cshtml");
        }

        // ── Completar Perfil (POST) ─────────────────────────────
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CompletarPerfil(CompletarPerfilDto dto)
        {
            if (!dto.AceitouTermos)
                ModelState.AddModelError("AceitouTermos", "Você precisa aceitar os termos de responsabilidade.");

            if (!ModelState.IsValid)
                return View("~/Views/Dashboard/Form.cshtml", dto);

            var appUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            await _usuarioService.AtualizarDadosDoadorAsync(appUserId, dto);

            TempData["Sucesso"] = "Perfil habilitado com sucesso!";
            return RedirectToAction("Dashboard", "Dashboard");   // ou "Index", dependendo da sua rota atual
        }
    }
}