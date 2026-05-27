using LigaDosPets.Application.Abstractions.Services;
using LigaDosPets.Application.DTOs.Usuario;
using LigaDosPets.Infrastructure.Identity;
using LigaDosPets.Web.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace LigaDosPets.Web.Controllers
{
    public class AuthController : Controller
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUsuarioService _usuarioService;

        public AuthController(
            SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager,
            IUsuarioService usuarioService)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _usuarioService = usuarioService;
        }

        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginModel model, string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            if (!ModelState.IsValid)
                return View(model);

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "E-mail ou senha inválidos.");
                return View(model);
            }

            var result = await _signInManager.PasswordSignInAsync(user, model.Senha, model.LembrarMe, lockoutOnFailure: true);
            if (result.Succeeded)
            {
                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    return LocalRedirect(returnUrl);

                return RedirectToAction("Index", "Home");
            }
            if (result.IsLockedOut)
                ModelState.AddModelError(string.Empty, "Conta bloqueada. Tente novamente mais tarde.");
            else
                ModelState.AddModelError(string.Empty, "E-mail ou senha inválidos.");

            return View(model);
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            if (model.Senha != model.ConfirmarSenha)
            {
                ModelState.AddModelError(string.Empty, "As senhas não conferem.");
                return View(model);
            }

            try
            {
                var dto = new RegistroUsuarioDto
                {
                    Nome = model.Nome,
                    Email = model.Email,
                    Senha = model.Senha,
                    Telefone = null
                };

                await _usuarioService.RegistrarUsuarioExternoAsync(dto);

                TempData["Sucesso"] = "Cadastro realizado com sucesso! Faça login para continuar.";
                return RedirectToAction("Login");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}