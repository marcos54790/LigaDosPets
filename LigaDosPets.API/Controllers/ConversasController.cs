using LigaDosPets.Application.Abstractions.Services;
using LigaDosPets.Application.DTOs.Usuario;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LigaDosPets.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ConversasController : ControllerBase
    {
        private readonly IConversaService _conversaService;
        private readonly IUsuarioService _usuarioService;

        public ConversasController(IConversaService conversaService, IUsuarioService usuarioService)
        {
            _conversaService = conversaService;
            _usuarioService = usuarioService;
        }

        [HttpGet]
        public async Task<IActionResult> GetMinhasConversas()
        {
            var usuario = await GetUsuarioLogado();
            if (usuario == null) return Unauthorized();

            var conversas = await _conversaService.GetConversasUsuarioAsync(usuario.Id);
            return Ok(conversas);
        }

        [HttpPost("iniciar")]
        public async Task<IActionResult> IniciarConversa([FromBody] IniciarConversaDto dto)
        {
            var usuario = await GetUsuarioLogado();
            if (usuario == null) return Unauthorized();

            var conversa = await _conversaService.IniciarOuObterConversaAsync(usuario.Id, dto.OutroUsuarioId, dto.AnimalId);
            return Ok(conversa);
        }

        [HttpGet("{conversaId}/mensagens")]
        public async Task<IActionResult> GetMensagens(int conversaId)
        {
            var usuario = await GetUsuarioLogado();
            if (usuario == null) return Unauthorized();

            var mensagens = await _conversaService.GetMensagensConversaAsync(conversaId, usuario.Id);
            return Ok(mensagens);
        }

        [HttpPost("{conversaId}/mensagens")]
        public async Task<IActionResult> EnviarMensagem(int conversaId, [FromBody] EnviarMensagemDto dto)
        {
            var usuario = await GetUsuarioLogado();
            if (usuario == null) return Unauthorized();

            var mensagem = await _conversaService.EnviarMensagemAsync(conversaId, usuario.Id, dto.Conteudo);
            return Ok(mensagem);
        }

        [HttpPost("{conversaId}/marcar-lidas")]
        public async Task<IActionResult> MarcarComoLidas(int conversaId)
        {
            var usuario = await GetUsuarioLogado();
            if (usuario == null) return Unauthorized();

            await _conversaService.MarcarMensagensComoLidasAsync(conversaId, usuario.Id);
            return Ok();
        }

        private async Task<UsuarioPerfilDto?> GetUsuarioLogado()
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId)) return null;
            return await _usuarioService.GetPerfilPorApplicationUserIdAsync(userId);
        }
    }

    // DTOs auxiliares (podem ficar em arquivos separados)
    public class IniciarConversaDto
    {
        public int OutroUsuarioId { get; set; }
        public int? AnimalId { get; set; }
    }

    public class EnviarMensagemDto
    {
        public string Conteudo { get; set; } = string.Empty;
    }

}