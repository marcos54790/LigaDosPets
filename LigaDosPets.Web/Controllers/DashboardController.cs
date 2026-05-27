using LigaDosPets.Application.Abstractions.Services;
using LigaDosPets.Application.DTOs.Animal;
using LigaDosPets.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LigaDosPets.Web.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly IAnimalService _animalService;
        private readonly IUsuarioService _usuarioService;
        private readonly IEspecieService _especieService;
        private readonly IPorteService _porteService;
        private readonly ICaracteristicaService _caracteristicaService;
        private readonly IRacaService _racaService;

        public DashboardController(
            IAnimalService animalService,
            IUsuarioService usuarioService,
            IEspecieService especieService,
            IPorteService porteService,
            ICaracteristicaService caracteristicaService,
            IRacaService racaService)
        {
            _animalService = animalService;
            _usuarioService = usuarioService;
            _especieService = especieService;
            _porteService = porteService;
            _caracteristicaService = caracteristicaService;
            _racaService = racaService;
        }

        // GET /Dashboard (listagem dos animais do usuário)
        public async Task<IActionResult> Dashboard()
        {
            var appUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var perfil = await _usuarioService.GetPerfilPorApplicationUserIdAsync(appUserId!);

            // Se o perfil do doador não foi completado, redireciona
            if (perfil == null || !perfil.PerfilDoadorCompleto)
                return RedirectToAction("CompletarPerfil", "Usuario");

            var usuarioId = perfil.Id;
            var animais = await _animalService.GetAnimaisPorUsuarioAsync(usuarioId);
            var model = animais.Select(a => new AnimalModel
            {
                Id = a.Id,
                Nome = a.Nome,
                Idade = a.Idade,
                EspecieNome = a.Especie,
                RacaNome = a.Raca ?? "SRD",
                FotoUrl = a.FotoPrincipalUrl ?? "/images/pets/placeholder.jpg",
                Sexo = a.Sexo,
                DisponivelParaAdocao = a.DisponivelParaAdocao,
                AguardandoValidacao = a.AguardandoValidacao
            }).ToList();

            return View(model); // View = Dashboard/Index.cshtml
        }

        // GET /Dashboard/Create (formulário completo)
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await CarregarDropdowns();
            return View(new CreateAnimalDto()); // View = Dashboard/Create.cshtml
        }

        // POST /Dashboard/Create (envio do formulário)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateAnimalDto dto)
        {
            if (!ModelState.IsValid)
            {
                await CarregarDropdowns();
                return View(dto);
            }

            var usuarioId = await GetUsuarioId();
            await _animalService.CreateAsync(dto, usuarioId, isAdmin: false);

            TempData["Sucesso"] = "Anúncio enviado para análise!";
            return RedirectToAction(nameof(Dashboard));   // 👈 antes estava nameof(Index)
        }

        // Endpoint para carregar raças dinamicamente
        [HttpGet]
        public async Task<IActionResult> ObterRacasPorEspecie(int especieId)
        {
            var racas = await _racaService.GetByEspecieAsync(especieId);
            var resultado = racas.Select(r => new { id = r.Id, nome = r.Nome });
            return Json(resultado);
        }

        private async Task<int> GetUsuarioId()
        {
            var appUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(appUserId)) throw new UnauthorizedAccessException();
            var perfil = await _usuarioService.GetPerfilPorApplicationUserIdAsync(appUserId);
            return perfil!.Id;
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
    }
}