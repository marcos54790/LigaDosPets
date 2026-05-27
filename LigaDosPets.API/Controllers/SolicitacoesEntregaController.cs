using LigaDosPets.Application.Abstractions.Services;
using LigaDosPets.Application.DTOs.SolicitacaoEntrega;
using LigaDosPets.Application.DTOs.Usuario;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LigaDosPets.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class SolicitacoesEntregaController : ControllerBase
    {
        private readonly ISolicitacaoEntregaAppService _solicitacaoService;
        private readonly IUsuarioService _usuarioService;

        public SolicitacoesEntregaController(
            ISolicitacaoEntregaAppService solicitacaoService,
            IUsuarioService usuarioService)
        {
            _solicitacaoService = solicitacaoService;
            _usuarioService = usuarioService;
        }

        [HttpGet]
        public async Task<IActionResult> GetMinhasSolicitacoes()
        {
            var usuario = await GetUsuarioLogado();
            if (usuario == null) return Unauthorized();

            var solicitacoes = await _solicitacaoService.GetSolicitacoesUsuarioAsync(usuario.Id);
            return Ok(solicitacoes);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var solicitacao = await _solicitacaoService.GetByIdAsync(id);
            if (solicitacao == null) return NotFound();
            return Ok(solicitacao);
        }

        [HttpPost]
        public async Task<IActionResult> Criar([FromBody] CreateSolicitacaoDto dto)
        {
            var usuario = await GetUsuarioLogado();
            if (usuario == null) return Unauthorized();

            var solicitacao = await _solicitacaoService.CriarSolicitacaoAsync(dto, usuario.Id);
            return CreatedAtAction(nameof(GetById), new { id = solicitacao.Id }, solicitacao);
        }

        [HttpGet("pendentes")]
        [Authorize(Roles = "Administrador,Gerente")]
        public async Task<IActionResult> GetPendentes()
        {
            var pendentes = await _solicitacaoService.GetSolicitacoesPendentesAsync();
            return Ok(pendentes);
        }

        [HttpPost("{id}/aprovar")]
        [Authorize(Roles = "Administrador,Gerente")]
        public async Task<IActionResult> Aprovar(int id, [FromBody] AprovarSolicitacaoDto dto)
        {
            var admin = await GetUsuarioLogado();
            if (admin == null) return Unauthorized();

            var solicitacao = await _solicitacaoService.AprovarSolicitacaoAsync(id, admin.Id, dto);
            return Ok(solicitacao);
        }

        [HttpPost("{id}/rejeitar")]
        [Authorize(Roles = "Administrador,Gerente")]
        public async Task<IActionResult> Rejeitar(int id, [FromBody] string motivo)
        {
            var admin = await GetUsuarioLogado();
            if (admin == null) return Unauthorized();

            var solicitacao = await _solicitacaoService.RejeitarSolicitacaoAsync(id, admin.Id, motivo);
            return Ok(solicitacao);
        }

        private async Task<UsuarioPerfilDto?> GetUsuarioLogado()
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId)) return null;
            return await _usuarioService.GetPerfilPorApplicationUserIdAsync(userId);
        }
    }
}