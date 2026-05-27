using LigaDosPets.Application.Abstractions.Services;
using LigaDosPets.Application.DTOs.Usuario;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LigaDosPets.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class NotificacoesController : ControllerBase
    {
        private readonly INotificacaoService _notificacaoService;
        private readonly IUsuarioService _usuarioService;

        public NotificacoesController(INotificacaoService notificacaoService, IUsuarioService usuarioService)
        {
            _notificacaoService = notificacaoService;
            _usuarioService = usuarioService;
        }

        [HttpGet]
        public async Task<IActionResult> GetMinhasNotificacoes()
        {
            var usuario = await GetUsuarioLogado();
            if (usuario == null) return Unauthorized();

            var notificacoes = await _notificacaoService.GetNotificacoesUsuarioAsync(usuario.Id);
            return Ok(notificacoes);
        }

        [HttpGet("nao-lidas/count")]
        public async Task<IActionResult> GetContagemNaoLidas()
        {
            var usuario = await GetUsuarioLogado();
            if (usuario == null) return Unauthorized();

            var count = await _notificacaoService.ContarNaoLidasAsync(usuario.Id);
            return Ok(new { count });
        }

        [HttpPost("{id}/marcar-lida")]
        public async Task<IActionResult> MarcarComoLida(int id)
        {
            var notificacao = await _notificacaoService.MarcarComoLidaAsync(id);
            if (notificacao == null) return NotFound();
            return Ok(notificacao);
        }

        [HttpPost("marcar-todas-lidas")]
        public async Task<IActionResult> MarcarTodasComoLidas()
        {
            var usuario = await GetUsuarioLogado();
            if (usuario == null) return Unauthorized();

            await _notificacaoService.MarcarTodasComoLidasAsync(usuario.Id);
            return Ok();
        }

        private async Task<UsuarioPerfilDto?> GetUsuarioLogado()
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId)) return null;
            return await _usuarioService.GetPerfilPorApplicationUserIdAsync(userId);
        }
    }
}