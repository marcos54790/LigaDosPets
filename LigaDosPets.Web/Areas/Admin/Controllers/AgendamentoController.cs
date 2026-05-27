// Areas/Admin/Controllers/AgendamentosController.cs
using LigaDosPets.Application.Abstractions.Services;
using LigaDosPets.Application.DTOs.Adocao;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LigaDosPets.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Administrador, Gerente")]
    public class AgendamentosController : Controller
    {
        private readonly IAgendamentoService _agendamentoService;
        private readonly IAnimalService _animalService;
        private readonly IUsuarioService _usuarioService;
        private readonly IAdocaoService _adocaoService;

        public AgendamentosController(
            IAgendamentoService agendamentoService,
            IAnimalService animalService,
            IUsuarioService usuarioService,
            IAdocaoService adocaoService)
        {
            _agendamentoService = agendamentoService;
            _animalService = animalService;
            _usuarioService = usuarioService;
            _adocaoService = adocaoService;
        }

        private async Task<int> GetAdminId()
        {
            var appUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(appUserId)) return 1;
            var perfil = await _usuarioService.GetPerfilPorApplicationUserIdAsync(appUserId);
            return perfil?.Id ?? 1;
        }

        // GET /Admin/Agendamentos — Pendentes
        public async Task<IActionResult> Index()
        {
            var agendamentos = await _agendamentoService.GetAgendamentosPendentesAsync();
            ViewBag.Aba = "pendentes";
            return View(agendamentos);
        }

        // GET /Admin/Agendamentos/Futuros
        public async Task<IActionResult> Futuros()
        {
            var agendamentos = await _agendamentoService.GetAgendamentosFuturosAsync();
            ViewBag.Aba = "futuros";
            return View("Index", agendamentos);
        }

        // POST /Admin/Agendamentos/Confirmar/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Confirmar(int id)
        {
            try
            {
                var adminId = await GetAdminId();
                await _agendamentoService.ConfirmarAgendamentoAsync(id, adminId);
                TempData["Sucesso"] = "Agendamento confirmado com sucesso!";
            }
            catch (Exception ex)
            {
                TempData["Erro"] = ex.Message;
            }
            return RedirectToAction(nameof(Index));
        }

        // POST /Admin/Agendamentos/Realizar/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Realizar(int id, bool foiAdotado)
        {
            try
            {
                var agendamento = await _agendamentoService.GetByIdAsync(id);
                if (agendamento == null)
                    throw new InvalidOperationException("Agendamento não encontrado.");

                await _agendamentoService.RealizarAgendamentoAsync(id);

                if (foiAdotado)
                {
                    var dto = new RegistrarAdocaoDto
                    {
                        AnimalId = agendamento.AnimalId,
                        UsuarioAdotanteId = agendamento.UsuarioId,
                        TermosAssinados = true,
                        Observacao = $"Adoção realizada durante visita agendada em {agendamento.DataVisita:dd/MM/yyyy}"
                    };
                    await _adocaoService.RegistrarAdocaoAsync(dto, await GetAdminId());
                    TempData["Sucesso"] = $"Visita realizada e {agendamento.NomeAnimal} adotado! 🎉";
                }
                else
                {
                    TempData["Sucesso"] = "Visita marcada como realizada!";
                }
            }
            catch (Exception ex)
            {
                TempData["Erro"] = ex.Message;
            }
            return RedirectToAction(nameof(Index));
        }

        // POST /Admin/Agendamentos/Cancelar/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancelar(int id, string motivo)
        {
            try
            {
                var adminId = await GetAdminId();
                await _agendamentoService.CancelarAgendamentoAsync(
                    id, motivo ?? "Cancelado pelo administrador.", adminId);
                TempData["Sucesso"] = "Agendamento cancelado.";
            }
            catch (Exception ex)
            {
                TempData["Erro"] = ex.Message;
            }
            return RedirectToAction(nameof(Index));
        }
    }
}