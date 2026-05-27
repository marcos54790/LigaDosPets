using LigaDosPets.Application.Abstractions.Services;
using LigaDosPets.Application.DTOs.Especie;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LigaDosPets.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Administrador, Gerente")]
    public class EspeciesController : Controller
    {
        private readonly IEspecieService _especieService;

        public EspeciesController(IEspecieService especieService)
        {
            _especieService = especieService;
        }

        // ── GET /Admin/Especies ───────────────────────────────
        public async Task<IActionResult> Index()
        {
            var especies = await _especieService.GetAllAsync();
            return View(especies);
        }

        // ── GET /Admin/Especies/Create ────────────────────────
        [HttpGet]
        public IActionResult Create()
        {
            return View(new CreateEspecieDto());
        }

        // ── POST /Admin/Especies/Create ───────────────────────
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateEspecieDto dto)
        {
            if (!ModelState.IsValid)
                return View(dto);

            try
            {
                await _especieService.CreateAsync(dto);
                TempData["Sucesso"] = $"Espécie \"{dto.Nome}\" criada com sucesso!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(dto);
            }
        }

        // ── GET /Admin/Especies/Edit/{id} ─────────────────────
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var especie = await _especieService.GetByIdAsync(id);
            if (especie == null) return NotFound();

            ViewBag.EspecieId = id;
            ViewBag.EspecieNome = especie.Nome;

            // Carrega as raças vinculadas para exibir na view
            var racas = await _especieService.GetRacasPorEspecieAsync(id);
            ViewBag.Racas = racas;

            var dto = new UpdateEspecieDto { Nome = especie.Nome };
            return View(dto);
        }

        // ── POST /Admin/Especies/Edit/{id} ────────────────────
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, UpdateEspecieDto dto)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.EspecieId = id;
                ViewBag.EspecieNome = dto.Nome;
                return View(dto);
            }

            try
            {
                await _especieService.UpdateAsync(id, dto);
                TempData["Sucesso"] = "Espécie atualizada com sucesso!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                ViewBag.EspecieId = id;
                ViewBag.EspecieNome = dto.Nome;
                return View(dto);
            }
        }

        // ── POST /Admin/Especies/Delete/{id} ──────────────────
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _especieService.DeleteAsync(id);
                TempData["Sucesso"] = "Espécie removida com sucesso!";
            }
            catch (Exception ex)
            {
                TempData["Erro"] = ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }
    }
}