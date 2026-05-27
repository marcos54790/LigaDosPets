using LigaDosPets.Application.Abstractions.Services;
using LigaDosPets.Application.DTOs.Animal;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LigaDosPets.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Administrador, Gerente")]
    public class AnimaisController : Controller
    {
        private readonly IAnimalService _animalService;
        private readonly IEspecieService _especieService;
        private readonly IPorteService _porteService;
        private readonly IRacaService _racaService;
        private readonly ICaracteristicaService _caracteristicaService;
        private readonly IUsuarioService _usuarioService;

        public AnimaisController(
            IAnimalService animalService,
            IEspecieService especieService,
            IPorteService porteService,
            IRacaService racaService,
            ICaracteristicaService caracteristicaService,
            IUsuarioService usuarioService)
        {
            _animalService = animalService;
            _especieService = especieService;
            _porteService = porteService;
            _racaService = racaService;
            _caracteristicaService = caracteristicaService;
            _usuarioService = usuarioService;
        }

        private async Task CarregarDropdowns()
        {
            ViewBag.Especies = (await _especieService.GetAllAsync()).OrderBy(e => e.Nome).ToList();
            ViewBag.Portes = (await _porteService.GetAllAsync()).OrderBy(p => p.Nome).ToList();
            ViewBag.Caracteristicas = (await _caracteristicaService.GetAllAsync())
                .GroupBy(c => c.Nome).Select(g => g.First()).OrderBy(c => c.Nome).ToList();
        }

        // GET /Admin/Animais → TODOS os animais
        public async Task<IActionResult> Index()
        {
            var animais = await _animalService.GetAnimaisDisponiveisAsync(new AnimalFiltroDto());
            return View(animais);
        }

        // GET /Admin/Animais/Create
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await CarregarDropdowns();
            return View(new CreateAnimalDto());
        }

        // POST /Admin/Animais/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateAnimalDto dto)
        {
            if (!ModelState.IsValid)
            {
                await CarregarDropdowns();
                return View(dto);
            }

            var adminId = await GetAdminId();
            await _animalService.CreateAsync(dto, adminId, isAdmin: true);
            TempData["Sucesso"] = "Animal cadastrado e publicado com sucesso!";
            return RedirectToAction(nameof(Index));
        }

        // GET /Admin/Animais/Edit/{id}
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var animal = await _animalService.GetByIdAsync(id);
            if (animal == null) return NotFound();

            await CarregarDropdowns();

            int especieId = await ObterEspecieIdPorNome(animal.Especie);
            int porteId = await ObterPorteIdPorNome(animal.Porte);
            int? racaId = string.IsNullOrEmpty(animal.Raca) ? null : await ObterRacaIdPorNome(animal.Raca, especieId);

            var caracteristicasIds = new List<int>();
            if (animal.Caracteristicas != null)
            {
                var todasCaracteristicas = await _caracteristicaService.GetAllAsync();
                caracteristicasIds = todasCaracteristicas
                    .Where(c => animal.Caracteristicas.Contains(c.Nome))
                    .Select(c => c.Id)
                    .ToList();
            }

            ViewBag.RacasEspecie = await _racaService.GetByEspecieAsync(especieId);
            ViewBag.AnimalId = id;
            ViewBag.AnimalNome = animal.Nome;

            var dto = new UpdateAnimalDto
            {
                Nome = animal.Nome,
                Idade = animal.Idade ?? 0,
                Sexo = animal.Sexo,
                Descricao = animal.Descricao ?? string.Empty,
                PorteId = porteId,
                EspecieId = especieId,
                RacaId = racaId,
                CaracteristicasIds = caracteristicasIds
            };

            return View(dto);
        }

        // POST /Admin/Animais/Edit/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, UpdateAnimalDto dto)
        {
            if (!ModelState.IsValid)
            {
                await CarregarDropdowns();
                ViewBag.AnimalId = id;
                return View(dto);
            }

            await _animalService.UpdateAsync(id, dto);
            TempData["Sucesso"] = "Animal atualizado com sucesso!";
            return RedirectToAction(nameof(Index));
        }

        // POST /Admin/Animais/Delete/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            await _animalService.DeleteAsync(id);
            TempData["Sucesso"] = "Animal removido.";
            return RedirectToAction(nameof(Index));
        }

        // GET /Admin/Animais/ObterRacasPorEspecie (endpoint para JS)
        [HttpGet]
        public async Task<IActionResult> ObterRacasPorEspecie(int especieId)
        {
            var racas = await _racaService.GetByEspecieAsync(especieId);
            return Json(racas.Select(r => new { id = r.Id, nome = r.Nome }));
        }

        // Helpers privados
        private async Task<int> GetAdminId()
        {
            var appUserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(appUserId)) return 1;
            var perfil = await _usuarioService.GetPerfilPorApplicationUserIdAsync(appUserId);
            return perfil?.Id ?? 1;
        }

        private async Task<int> ObterEspecieIdPorNome(string nome)
        {
            var especies = await _especieService.GetAllAsync();
            return especies.FirstOrDefault(e => e.Nome == nome)?.Id ?? 0;
        }

        private async Task<int> ObterPorteIdPorNome(string nome)
        {
            var portes = await _porteService.GetAllAsync();
            return portes.FirstOrDefault(p => p.Nome == nome)?.Id ?? 0;
        }

        private async Task<int?> ObterRacaIdPorNome(string nome, int especieId)
        {
            var racas = await _racaService.GetByEspecieAsync(especieId);
            return racas.FirstOrDefault(r => r.Nome == nome)?.Id;
        }
    }
}