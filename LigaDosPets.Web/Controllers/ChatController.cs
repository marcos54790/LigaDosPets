using LigaDosPets.Application.Abstractions.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LigaDosPets.Web.Controllers
{
    [Authorize]
    public class ChatController : Controller
    {
        private readonly IConversaService _conversaService;
        private readonly IUsuarioService _usuarioService;

        public ChatController(IConversaService conversaService, IUsuarioService usuarioService)
        {
            _conversaService = conversaService;
            _usuarioService = usuarioService;
        }

        private async Task<int> GetUsuarioId()
        {
            var appUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(appUserId)) throw new UnauthorizedAccessException();
            var perfil = await _usuarioService.GetPerfilPorApplicationUserIdAsync(appUserId);
            return perfil?.Id ?? throw new UnauthorizedAccessException();
        }

        // GET /Chat (lista de conversas)
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var usuarioId = await GetUsuarioId();
            var conversas = await _conversaService.GetConversasUsuarioAsync(usuarioId);
            ViewBag.UsuarioId = usuarioId;
            return View("~/Views/Atendimento/Index.cshtml", conversas);
        }

        // GET /Chat/Iniciar
        [HttpGet]
        public async Task<IActionResult> Iniciar(int? animalId = null)
        {
            // Impede que administradores usem o chat público
            if (User.IsInRole("Administrador") || User.IsInRole("Gerente"))
            {
                TempData["Erro"] = "Administradores não utilizam o chat público. Acesse a Central de Mensagens.";
                return RedirectToAction("Index", "Conversas", new { area = "Admin" });
            }

            var usuarioId = await GetUsuarioId();
            var admin = await _usuarioService.GetAdminPrincipalAsync();
            if (admin == null)
            {
                TempData["Erro"] = "Nenhum administrador disponível no momento.";
                return RedirectToAction("Index");
            }

            var conversa = await _conversaService.IniciarOuObterConversaAsync(usuarioId, admin.Id, animalId);
            return RedirectToAction("Conversa", new { id = conversa.Id });
        }

        // GET /Chat/Conversa/{id}
        [HttpGet]
        public async Task<IActionResult> Conversa(int id)
        {
            var usuarioId = await GetUsuarioId();
            var mensagens = await _conversaService.GetMensagensConversaAsync(id, usuarioId);

            ViewBag.ConversaId = id;
            ViewBag.UsuarioId = usuarioId;

            return View("~/Views/Atendimento/Conversa.cshtml", mensagens);
        }

        // GET /Chat/MensagensJson/{conversaId}
        [HttpGet]
        public async Task<IActionResult> MensagensJson(int conversaId)
        {
            var usuarioId = await GetUsuarioId();
            var mensagens = await _conversaService.GetMensagensConversaAsync(conversaId, usuarioId);
            return Json(mensagens.Select(m => new
            {
                id = m.Id,
                remetenteId = m.RemetenteId,
                nomeRemetente = m.NomeRemetente,
                conteudo = m.Conteudo,
                dataEnvio = m.DataEnvio.ToString("HH:mm"),
                meuId = usuarioId
            }));
        }
    }
}