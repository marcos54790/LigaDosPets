using LigaDosPets.Application.Abstractions.Services;
using LigaDosPets.Application.DTOs.Animal;
using LigaDosPets.Application.DTOs.SolicitacaoEntrega;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LigaDosPets.Web.Controllers
{
    [Authorize]
    public class SolicitacoesController : Controller
    {
        private readonly ISolicitacaoEntregaAppService _solicitacaoService;
        private readonly IUsuarioService _usuarioService;
        private readonly IEspecieService _especieService;
        private readonly IPorteService _porteService;
        private readonly ICaracteristicaService _caracteristicaService;
        private readonly IRacaService _racaService;

        public SolicitacoesController(
            ISolicitacaoEntregaAppService solicitacaoService,
            IUsuarioService usuarioService,
            IEspecieService especieService,
            IPorteService porteService,
            ICaracteristicaService caracteristicaService,
            IRacaService racaService)
        {
            _solicitacaoService = solicitacaoService;
            _usuarioService = usuarioService;
            _especieService = especieService;
            _porteService = porteService;
            _caracteristicaService = caracteristicaService;
            _racaService = racaService;
        }

        private async Task<int> GetUsuarioId()
        {
            var appUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var perfil = await _usuarioService.GetPerfilPorApplicationUserIdAsync(appUserId!);
            return perfil!.Id;
        }

        // GET /Solicitacoes (lista)
        public async Task<IActionResult> Index()
        {
            var usuarioId = await GetUsuarioId();
            var solicitacoes = await _solicitacaoService.GetSolicitacoesUsuarioAsync(usuarioId);
            return View(solicitacoes);
        }

        // GET /Solicitacoes/Create (formulário completo)
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await CarregarDropdowns();
            return View(new CreateAnimalDto());
        }

        // POST /Solicitacoes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateAnimalDto dto)
        {
            if (!ModelState.IsValid)
            {
                await CarregarDropdowns();
                return View(dto);
            }

            // Mapeia CreateAnimalDto → CreateSolicitacaoDto
            var solicitacaoDto = new CreateSolicitacaoDto
            {
                NomeAnimal = dto.Nome,
                DescricaoAnimal = dto.Descricao,
                MotivoEntrega = dto.MotivoDoacao,
                EspecieDescricao = await ObterNomeEspecie(dto.EspecieId),
                RacaDescricao = dto.RacaId.HasValue ? await ObterNomeRaca(dto.RacaId.Value) : null,
                IdadeEstimada = dto.Idade > 0 ? $"{dto.Idade} ano(s)" : null,
                PorteDescricao = await ObterNomePorte(dto.PorteId),
                CaracteristicasDescricao = await ObterNomesCaracteristicas(dto.CaracteristicasIds),
                Fotos = dto.Fotos
            };

            var usuarioId = await GetUsuarioId();
            await _solicitacaoService.CriarSolicitacaoAsync(solicitacaoDto, usuarioId);

            TempData["Sucesso"] = "Solicitação enviada com sucesso! Aguardando análise.";
            return RedirectToAction(nameof(Index));
        }

        private async Task CarregarDropdowns()
        {
            var especies = await _especieService.GetAllAsync();
            ViewBag.Especies = especies.OrderBy(e => e.Nome).ToList();

            var portes = await _porteService.GetAllAsync();
            ViewBag.Portes = portes.OrderBy(p => p.Nome).ToList();

            var caracteristicas = await _caracteristicaService.GetAllAsync();
            ViewBag.Caracteristicas = caracteristicas
                .GroupBy(c => c.Nome)
                .Select(g => g.First())
                .OrderBy(c => c.Nome)
                .ToList();
        }

        private async Task<string?> ObterNomeEspecie(int especieId)
        {
            var especies = await _especieService.GetAllAsync();
            return especies.FirstOrDefault(e => e.Id == especieId)?.Nome;
        }

        private async Task<string?> ObterNomeRaca(int racaId)
        {
            var racas = await _racaService.GetAllAsync();
            return racas.FirstOrDefault(r => r.Id == racaId)?.Nome;
        }

        private async Task<string?> ObterNomePorte(int porteId)
        {
            var portes = await _porteService.GetAllAsync();
            return portes.FirstOrDefault(p => p.Id == porteId)?.Nome;
        }

        private async Task<string> ObterNomesCaracteristicas(List<int> ids)
        {
            if (ids == null || !ids.Any()) return string.Empty;
            var todas = await _caracteristicaService.GetAllAsync();
            var nomes = todas.Where(c => ids.Contains(c.Id)).Select(c => c.Nome);
            return string.Join(", ", nomes);
        }
    }
}