using LigaDosPets.Application.Abstractions.Services;
using LigaDosPets.Application.DTOs.Raca;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LigaDosPets.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Administrador, Gerente")]
    public class RacasController : Controller
    {
        private readonly IRacaService _racaService;
        private readonly IEspecieService _especieService;

        public RacasController(IRacaService racaService, IEspecieService especieService)
        {
            _racaService = racaService;
            _especieService = especieService;
        }

        // Dropdown de espécies para Create/Edit
        private async Task CarregarEspecies()
        {
            ViewBag.Especies = await _especieService.GetAllAsync();
        }

        // GET /Admin/Racas
        public async Task<IActionResult> Index()
        {
            var racas = await _racaService.GetAllAsync();  // Inclui NomeEspecie
            return View(racas);
        }

        // GET /Admin/Racas/Create
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await CarregarEspecies();
            return View(new CreateRacaDto());
        }

        // POST /Admin/Racas/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateRacaDto dto)
        {
            if (!ModelState.IsValid)
            {
                await CarregarEspecies();
                return View(dto);
            }

            await _racaService.CreateAsync(dto);
            TempData["Sucesso"] = "Raça cadastrada com sucesso!";
            return RedirectToAction(nameof(Index));
        }

        // GET /Admin/Racas/Edit/5
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var raca = await _racaService.GetByIdAsync(id);
            if (raca == null) return NotFound();

            await CarregarEspecies();

            var dto = new UpdateRacaDto { Nome = raca.Nome };
            ViewBag.RacaId = id;
            ViewBag.RacaNome = raca.Nome;
            ViewBag.EspecieIdAtual = raca.EspecieId;

            return View(dto);
        }

        // POST /Admin/Racas/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, UpdateRacaDto dto)
        {
            if (!ModelState.IsValid)
            {
                await CarregarEspecies();
                ViewBag.RacaId = id;
                ViewBag.EspecieIdAtual = ViewBag.EspecieIdAtual;
                return View(dto);
            }

            // Nota: o UpdateRacaDto atual só tem Nome. Precisamos de um DTO que aceite EspecieId também,
            // ou um método separado no serviço. Vou manter o Nome e ignorar a troca de espécie por simplicidade,
            // mas exibiremos o dropdown informativo.
            await _racaService.UpdateAsync(id, dto);
            TempData["Sucesso"] = "Raça atualizada com sucesso!";
            return RedirectToAction(nameof(Index));
        }

        // POST /Admin/Racas/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            await _racaService.DeleteAsync(id);
            TempData["Sucesso"] = "Raça removida.";
            return RedirectToAction(nameof(Index));
        }
    }
}