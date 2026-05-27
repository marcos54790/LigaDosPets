// Areas/Admin/Controllers/UsuariosController.cs
using LigaDosPets.Application.Abstractions.Repositories;
using LigaDosPets.Application.Abstractions.Services;
using LigaDosPets.Application.DTOs.Usuario;
using LigaDosPets.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LigaDosPets.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Administrador, Gerente")]   // acesso base: ambos
    public class UsuariosController : Controller
    {
        private readonly IUsuarioService _usuarioService;
        private readonly ITipoUsuarioRepository _tipoUsuarioRepo;

        public UsuariosController(
            IUsuarioService usuarioService,
            ITipoUsuarioRepository tipoUsuarioRepo)
        {
            _usuarioService = usuarioService;
            _tipoUsuarioRepo = tipoUsuarioRepo;
        }

        private async Task<int> GetUsuarioLogadoId()
        {
            var appUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(appUserId)) return 0;
            var perfil = await _usuarioService.GetPerfilPorApplicationUserIdAsync(appUserId);
            return perfil?.Id ?? 0;
        }

        // ── GET /Admin/Usuarios ────────────────────────────────────────────────
        // Ambos podem listar
        public async Task<IActionResult> Index()
        {
            var usuarios = await _usuarioService.GetAllAsync();
            return View(usuarios);
        }

        // ── GET /Admin/Usuarios/Create ─────────────────────────────────────────
        // Só Admin cria usuários diretamente
        [Authorize(Roles = "Administrador")]
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var tipos = await _tipoUsuarioRepo.GetAllAsync();
            ViewBag.TiposUsuarios = tipos.Select(t => new { t.Id, t.Nome }).ToList();
            return View(new UsuarioViewModel());
        }

        // ── POST /Admin/Usuarios/Create ────────────────────────────────────────
        [Authorize(Roles = "Administrador")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UsuarioViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var tipos = await _tipoUsuarioRepo.GetAllAsync();
                ViewBag.TiposUsuarios = tipos.Select(t => new { t.Id, t.Nome }).ToList();
                return View(model);
            }

            try
            {
                var dto = new RegistroUsuarioDto
                {
                    Nome = model.Nome,
                    Email = model.Email,
                    Senha = model.Senha,
                    Telefone = model.Telefone
                };

                await _usuarioService.CriarUsuarioAsync(dto, model.TipoUsuarioId);
                TempData["Sucesso"] = "Usuário criado com sucesso!";
                return RedirectToAction(nameof(Index));
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                var tipos = await _tipoUsuarioRepo.GetAllAsync();
                ViewBag.TiposUsuarios = tipos.Select(t => new { t.Id, t.Nome }).ToList();
                return View(model);
            }
        }

        // ── GET /Admin/Usuarios/Edit/{id} ──────────────────────────────────────
        // Ambos podem editar nome/telefone (campos básicos)
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var logadoId = await GetUsuarioLogadoId();
            if (id == logadoId)
            {
                TempData["Erro"] = "Edite seu próprio perfil pela área do usuário.";
                return RedirectToAction(nameof(Index));
            }

            var usuario = await _usuarioService.GetPerfilAsync(id);
            if (usuario == null) return NotFound();

            ViewBag.UsuarioId = id;
            ViewBag.UsuarioNome = usuario.Nome;
            ViewBag.UsuarioEmail = usuario.Email;
            ViewBag.Ativo = usuario.Ativo;
            ViewBag.TipoUsuario = usuario.TipoUsuario;
            ViewBag.IsAdmin = User.IsInRole("Administrador");

            return View(new UpdateUsuarioDto
            {
                Nome = usuario.Nome,
                Telefone = usuario.Telefone
            });
        }

        // ── POST /Admin/Usuarios/Edit/{id} ─────────────────────────────────────
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, UpdateUsuarioDto dto)
        {
            var logadoId = await GetUsuarioLogadoId();
            if (id == logadoId)
            {
                TempData["Erro"] = "Edite seu próprio perfil pela área do usuário.";
                return RedirectToAction(nameof(Index));
            }

            if (!ModelState.IsValid)
            {
                ViewBag.UsuarioId = id;
                ViewBag.IsAdmin = User.IsInRole("Administrador");
                return View(dto);
            }

            await _usuarioService.AtualizarPerfilAsync(id, dto);
            TempData["Sucesso"] = "Usuário atualizado com sucesso!";
            return RedirectToAction(nameof(Index));
        }

        // ── POST /Admin/Usuarios/Promover/{id} ─────────────────────────────────
        // SOMENTE Administrador pode mudar roles
        [Authorize(Roles = "Administrador")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Promover(int id, string novaRole)
        {
            var logadoId = await GetUsuarioLogadoId();
            if (id == logadoId)
            {
                TempData["Erro"] = "Você não pode alterar sua própria role.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                await _usuarioService.PromoverUsuarioAsync(id, novaRole);
                TempData["Sucesso"] = $"Usuário promovido para {novaRole} com sucesso!";
            }
            catch (InvalidOperationException ex)
            {
                TempData["Erro"] = ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }

        // ── POST /Admin/Usuarios/Desativar/{id} ────────────────────────────────
        // Ambos podem desativar usuários comuns; Admin pode desativar qualquer um
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Desativar(int id)
        {
            var logadoId = await GetUsuarioLogadoId();
            if (id == logadoId)
            {
                TempData["Erro"] = "Você não pode desativar a si mesmo!";
                return RedirectToAction(nameof(Index));
            }

            // Gerente não pode desativar Admin ou outro Gerente
            if (!User.IsInRole("Administrador"))
            {
                var alvo = await _usuarioService.GetPerfilAsync(id);
                if (alvo?.TipoUsuario is "Administrador" or "Gerente")
                {
                    TempData["Erro"] = "Gerentes só podem desativar usuários comuns.";
                    return RedirectToAction(nameof(Index));
                }
            }

            await _usuarioService.DesativarUsuarioAsync(id);
            TempData["Sucesso"] = "Usuário desativado.";
            return RedirectToAction(nameof(Index));
        }

        // ── POST /Admin/Usuarios/Ativar/{id} ───────────────────────────────────
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Ativar(int id)
        {
            var logadoId = await GetUsuarioLogadoId();
            if (id == logadoId)
            {
                TempData["Erro"] = "Você não pode ativar a si mesmo!";
                return RedirectToAction(nameof(Index));
            }

            if (!User.IsInRole("Administrador"))
            {
                var alvo = await _usuarioService.GetPerfilAsync(id);
                if (alvo?.TipoUsuario is "Administrador" or "Gerente")
                {
                    TempData["Erro"] = "Gerentes só podem reativar usuários comuns.";
                    return RedirectToAction(nameof(Index));
                }
            }

            await _usuarioService.AtivarUsuarioAsync(id);
            TempData["Sucesso"] = "Usuário ativado.";
            return RedirectToAction(nameof(Index));
        }

        // ── POST /Admin/Usuarios/Delete/{id} ───────────────────────────────────
        // SOMENTE Administrador pode excluir permanentemente
        [Authorize(Roles = "Administrador")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var logadoId = await GetUsuarioLogadoId();
            if (id == logadoId)
            {
                TempData["Erro"] = "Você não pode excluir a si mesmo!";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                await _usuarioService.ExcluirUsuarioAsync(id);
                TempData["Sucesso"] = "Usuário excluído permanentemente.";
            }
            catch (InvalidOperationException ex)
            {
                TempData["Erro"] = ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }
    }
}