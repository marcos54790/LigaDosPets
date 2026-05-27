using LigaDosPets.Application.Abstractions.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LigaDosPets.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Administrador, Gerente")]
    public class SolicitacoesController : Controller
    {
        private readonly ISolicitacaoEntregaAppService _solicitacaoService;
        private readonly IEspecieService _especieService;
        private readonly IPorteService _porteService;
        private readonly ICaracteristicaService _caracteristicaService;
        private readonly IUsuarioService _usuarioService;  // ← adicionado

        public SolicitacoesController(
            ISolicitacaoEntregaAppService solicitacaoService,
            IEspecieService especieService,
            IPorteService porteService,
            ICaracteristicaService caracteristicaService,
            IUsuarioService usuarioService)             // ← injetado
        {
            _solicitacaoService = solicitacaoService;
            _especieService = especieService;
            _porteService = porteService;
            _caracteristicaService = caracteristicaService;
            _usuarioService = usuarioService;
        }

        public async Task<IActionResult> Index()
        {
            var pendentes = await _solicitacaoService.GetSolicitacoesPendentesAsync();
            return View(pendentes);
        }

        public async Task<IActionResult> Detalhes(int id)
        {
            var solicitacao = await _solicitacaoService.GetByIdAsync(id);
            if (solicitacao == null) return NotFound();

            if (solicitacao.UsuarioId > 0)
            {
                var usuario = await _usuarioService.GetPerfilAsync(solicitacao.UsuarioId);
                if (usuario != null)
                {
                    solicitacao.EmailUsuario = usuario.Email;
                    solicitacao.TelefoneUsuario = usuario.Telefone;
                    solicitacao.CepUsuario = usuario.Cep;
                }
            }

            return View(solicitacao);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Aprovar(int id)
        {
            await _solicitacaoService.AprovarSolicitacaoSimplesAsync(id, null);
            TempData["Sucesso"] = "Solicitação aprovada e animal cadastrado no catálogo!";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Rejeitar(int id, string motivo)
        {
            await _solicitacaoService.RejeitarSolicitacaoAsync(id, 0, motivo);
            TempData["Sucesso"] = "Solicitação rejeitada.";
            return RedirectToAction(nameof(Index));
        }
    }
}