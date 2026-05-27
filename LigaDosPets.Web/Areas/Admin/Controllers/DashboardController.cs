using LigaDosPets.Application.Abstractions.Services;
using LigaDosPets.Application.DTOs.Animal;
using LigaDosPets.Web.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LigaDosPets.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Administrador, Gerente")]
    public class DashboardController : Controller
    {
        private readonly IAnimalService _animalService;
        private readonly IAdocaoService _adocaoService;
        private readonly IEspecieService _especieService;
        private readonly IPorteService _porteService;
        private readonly IUsuarioService _usuarioService;
        private readonly IAgendamentoService _agendamentoService;
        private readonly ISolicitacaoEntregaAppService _solicitacaoService;

        public DashboardController(
            IAnimalService animalService,
            IAdocaoService adocaoService,
            IEspecieService especieService,
            IPorteService porteService,
            IUsuarioService usuarioService,
            IAgendamentoService agendamentoService,
            ISolicitacaoEntregaAppService solicitacaoService)
        {
            _animalService = animalService;
            _adocaoService = adocaoService;
            _especieService = especieService;
            _porteService = porteService;
            _usuarioService = usuarioService;
            _agendamentoService = agendamentoService;
            _solicitacaoService = solicitacaoService;
        }

        public async Task<IActionResult> Index()
        {
            var animaisDisponiveis = await _animalService.GetAnimaisDisponiveisAsync(new AnimalFiltroDto());
            var animaisPendentes = await _animalService.GetAnimaisPendentesValidacaoAsync();
            var adocoes = await _adocaoService.GetHistoricoAsync();
            var especies = await _especieService.GetAllAsync();
            var portes = await _porteService.GetAllAsync();
            var usuarios = await _usuarioService.GetAllAsync();
            var agendamentos = await _agendamentoService.GetAgendamentosPendentesAsync();
            var solicitacoes = await _solicitacaoService.GetSolicitacoesPendentesAsync();

            var listaAnimais = animaisDisponiveis.ToList();

            // ── Gráfico: Espécies ─────────────────────────────
            var labelsEspecies = new List<string>();
            var valoresEspecies = new List<int>();
            foreach (var e in especies)
            {
                var count = listaAnimais.Count(a => a.Especie == e.Nome);
                if (count > 0)
                {
                    labelsEspecies.Add(e.Nome);
                    valoresEspecies.Add(count);
                }
            }

            // ── Gráfico: Portes ───────────────────────────────
            var labelsPortes = new List<string>();
            var valoresPortes = new List<int>();
            foreach (var p in portes)
            {
                var count = listaAnimais.Count(a => a.Porte == p.Nome);
                labelsPortes.Add(p.Nome);
                valoresPortes.Add(count);
            }

            // ── Gráfico: Cadastros por mês (últimos 6 meses) ──
            var labelsMeses = new List<string>();
            var valoresCadastrosMes = new List<int>();
            var todoAnimais = listaAnimais.Concat(animaisPendentes).ToList();

            var hoje = DateTime.UtcNow;
            for (int i = 5; i >= 0; i--)
            {
                labelsMeses.Add(hoje.AddMonths(-i).ToString("MMM/yy"));
                valoresCadastrosMes.Add(0);
            }

            // ── Sexo ──────────────────────────────────────────
            var totalMachos = listaAnimais.Count(a => a.Sexo == "Macho");
            var totalFemeas = listaAnimais.Count(a => a.Sexo == "Fêmea");

            // ── Solicitações pendentes ────────────────────────
            var solicitacoesPendentes = solicitacoes.Count(s => s.Status == "Pendente");

            var model = new DashboardModel
            {
                TotalAnimais = listaAnimais.Count,
                TotalAdotados = adocoes.Count(),
                TotalPendentesValidacao = animaisPendentes.Count(),
                TotalUsuarios = usuarios.Count(),
                TotalAgendamentos = agendamentos.Count(),
                TotalSolicitacoesPendentes = solicitacoesPendentes,

                LabelsEspecies = labelsEspecies,
                ValoresEspecies = valoresEspecies,

                LabelsPortes = labelsPortes,
                ValoresPortes = valoresPortes,

                TotalMachos = totalMachos,
                TotalFemeas = totalFemeas,

                LabelsMeses = labelsMeses,
                ValoresCadastrosMes = valoresCadastrosMes,
            };

            return View(model);
        }
    }
}