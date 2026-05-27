using LigaDosPets.Application.Abstractions.Services;
using LigaDosPets.Application.DTOs.Caracteristica;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LigaDosPets.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Administrador, Gerente")]
    public class CaracteristicasController : Controller
    {
        private readonly ICaracteristicaService _caracteristicaService;

        public CaracteristicasController(ICaracteristicaService caracteristicaService)
        {
            _caracteristicaService = caracteristicaService;
        }

        // GET /Admin/Caracteristicas
        public async Task<IActionResult> Index()
        {
            var lista = await _caracteristicaService.GetAllAsync();
            return View(lista);
        }

        // GET /Admin/Caracteristicas/Create
        [HttpGet]
        public IActionResult Create()
        {
            return View(new CreateCaracteristicaDto());
        }

        // POST /Admin/Caracteristicas/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateCaracteristicaDto dto)
        {
            if (!ModelState.IsValid)
                return View(dto);

            await _caracteristicaService.CreateAsync(dto);
            TempData["Sucesso"] = "Característica criada com sucesso!";
            return RedirectToAction(nameof(Index));
        }

        // GET /Admin/Caracteristicas/Edit/5
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var item = await _caracteristicaService.GetByIdAsync(id);
            if (item == null)
                return NotFound();

            var model = new UpdateCaracteristicaDto { Nome = item.Nome };
            ViewBag.Nome = item.Nome;
            return View(model);
        }

        // POST /Admin/Caracteristicas/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, UpdateCaracteristicaDto dto)
        {
            if (!ModelState.IsValid)
                return View(dto);

            await _caracteristicaService.UpdateAsync(id, dto);
            TempData["Sucesso"] = "Característica atualizada!";
            return RedirectToAction(nameof(Index));
        }

        // POST /Admin/Caracteristicas/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            await _caracteristicaService.DeleteAsync(id);
            TempData["Sucesso"] = "Característica removida.";
            return RedirectToAction(nameof(Index));
        }
    }
}