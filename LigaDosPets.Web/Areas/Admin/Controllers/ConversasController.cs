using LigaDosPets.Application.Abstractions.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LigaDosPets.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Administrador, Gerente")]
    public class ConversasController : Controller
    {
        private readonly IConversaService _conversaService;
        private readonly IUsuarioService _usuarioService;

        public ConversasController(IConversaService conversaService, IUsuarioService usuarioService)
        {
            _conversaService = conversaService;
            _usuarioService = usuarioService;
        }

        private async Task<int> GetAdminId()
        {
            var appUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(appUserId)) throw new UnauthorizedAccessException();
            var perfil = await _usuarioService.GetPerfilPorApplicationUserIdAsync(appUserId);
            return perfil?.Id ?? throw new UnauthorizedAccessException();
        }

        // GET /Admin/Conversas → Central de mensagens do admin
        public async Task<IActionResult> Index()
        {
            var adminId = await GetAdminId();
            var conversas = await _conversaService.GetConversasUsuarioAsync(adminId);

            // Remove conversas onde o outro participante é o próprio admin
            conversas = conversas
                .Where(c => c.NomeOutroUsuario != "Administrador" && c.NomeOutroUsuario != "Admin")
                .ToList();

            ViewBag.AdminId = adminId;
            return View("~/Areas/Admin/Views/Suporte/Index.cshtml", conversas);
        }

        // GET /Admin/Conversas/Atender/{id} → Admin abre uma conversa
        public async Task<IActionResult> Atender(int id)
        {
            var adminId = await GetAdminId();
            var mensagens = await _conversaService.GetMensagensConversaAsync(id, adminId);

            // Busca o nome do outro participante
            var conversa = (await _conversaService.GetConversasUsuarioAsync(adminId))
                .FirstOrDefault(c => c.Id == id);
            ViewBag.OutroUsuarioNome = conversa?.NomeOutroUsuario ?? "Usuário";

            ViewBag.ConversaId = id;
            ViewBag.AdminId = adminId;

            return View("~/Areas/Admin/Views/Suporte/Atendimento.cshtml", mensagens);
        }

        // POST /Admin/Conversas/Finalizar/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Finalizar(int id)
        {
            await _conversaService.ExcluirConversaAsync(id);
            TempData["Sucesso"] = "Atendimento finalizado. A conversa foi encerrada.";
            return RedirectToAction(nameof(Index));
        }
    }
}