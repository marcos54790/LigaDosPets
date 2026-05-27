using LigaDosPets.Application.Abstractions.Services;
using LigaDosPets.Application.DTOs.Adocao;
using LigaDosPets.Application.DTOs.Usuario;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LigaDosPets.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class AdocoesController : ControllerBase
    {
        private readonly IAdocaoService _adocaoService;
        private readonly IUsuarioService _usuarioService;

        public AdocoesController(IAdocaoService adocaoService, IUsuarioService usuarioService)
        {
            _adocaoService = adocaoService;
            _usuarioService = usuarioService;
        }

        [HttpGet]
        [Authorize(Roles = "Administrador,Gerente")]
        public async Task<IActionResult> GetHistorico()
        {
            var historico = await _adocaoService.GetHistoricoAsync();
            return Ok(historico);
        }

        [HttpGet("minhas")]
        public async Task<IActionResult> GetMinhasAdocoes()
        {
            var usuario = await GetUsuarioLogado();
            if (usuario == null) return Unauthorized();

            var adocoes = await _adocaoService.GetAdocoesPorAdotanteAsync(usuario.Id);
            return Ok(adocoes);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var adocao = await _adocaoService.GetByIdAsync(id);
            if (adocao == null) return NotFound();
            return Ok(adocao);
        }

        [HttpPost]
        [Authorize(Roles = "Administrador,Gerente")]
        public async Task<IActionResult> Registrar([FromBody] RegistrarAdocaoDto dto)
        {
            var admin = await GetUsuarioLogado();
            if (admin == null) return Unauthorized();

            try
            {
                var adocao = await _adocaoService.RegistrarAdocaoAsync(dto, admin.Id);
                return CreatedAtAction(nameof(GetById), new { id = adocao.Id }, adocao);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("animal/{animalId}/pode-ser-adotado")]
        public async Task<IActionResult> AnimalPodeSerAdotado(int animalId)
        {
            var pode = await _adocaoService.AnimalPodeSerAdotadoAsync(animalId);
            return Ok(new { pode });
        }

        private async Task<UsuarioPerfilDto?> GetUsuarioLogado()
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId)) return null;
            return await _usuarioService.GetPerfilPorApplicationUserIdAsync(userId);
        }
    }
}