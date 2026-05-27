// Web/Controllers/DetailsController.cs
using LigaDosPets.Application.Abstractions.Services;
using LigaDosPets.Application.DTOs.Agendamento;
using LigaDosPets.Application.DTOs.Usuario;
using LigaDosPets.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LigaDosPets.Web.Controllers
{
    public class DetailsController : Controller
    {
        private readonly IAnimalService _animalService;
        private readonly IAgendamentoService _agendamentoService;
        private readonly IUsuarioService _usuarioService;

        public DetailsController(
            IAnimalService animalService,
            IAgendamentoService agendamentoService,
            IUsuarioService usuarioService)
        {
            _animalService = animalService;
            _agendamentoService = agendamentoService;
            _usuarioService = usuarioService;
        }

        // GET: /Details/{id}
        public async Task<IActionResult> Details(int id)
        {
            var animal = await _animalService.GetByIdAsync(id);
            if (animal == null) return NotFound();

            var model = new DetailsModel
            {
                Id = animal.Id,
                Nome = animal.Nome,
                Idade = animal.Idade,
                Sexo = animal.Sexo,
                Descricao = animal.Descricao,
                EspecieNome = animal.Especie,
                RacaNome = animal.Raca ?? "SRD",
                PorteNome = animal.Porte,
                UsuarioCadastroNome = animal.NomeUsuarioCadastro,
                DisponivelParaAdocao = animal.DisponivelParaAdocao,
                Caracteristicas = animal.Caracteristicas ?? new List<string>(),
                Fotos = animal.Fotos.Select(f => new FotoModel { Url = f }).ToList(),
                HorariosDisponiveis = new List<string> { "09:00", "10:00", "11:00", "14:00", "15:00", "16:00", "17:00", "18:00" },
            };

            return View(model);
        }

        // GET: /Details/HorariosOcupados?data=2026-05-10
        // Endpoint chamado via fetch quando o usuário troca a data no modal

        // POST: /Details/AgendarVisita
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AgendarVisita(int animalId, DateTime dataVisita, string horaVisita, string? observacao)
        {
            if (!User.Identity!.IsAuthenticated)
                return RedirectToAction("Login", "Auth");

            var usuario = await ObterUsuarioLogado();
            if (usuario == null)
                return RedirectToAction("Login", "Auth");

            try
            {
                var dataHora = dataVisita.Date + TimeSpan.Parse(horaVisita);

                var dto = new CreateAgendamentoDto
                {
                    AnimalId = animalId,
                    DataVisita = dataHora,
                    Observacao = observacao
                };

                await _agendamentoService.CriarAgendamentoAsync(dto, usuario.Id);

                TempData["Sucesso"] = $"Visita agendada para {dataHora:dd/MM/yyyy} às {dataHora:HH:mm}. Aguarde a confirmação!";
            }
            catch (InvalidOperationException ex)
            {
                TempData["Erro"] = ex.Message;
            }

            return RedirectToAction("Details", new { id = animalId });
        }

        private async Task<UsuarioPerfilDto?> ObterUsuarioLogado()
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim)) return null;
            return await _usuarioService.GetPerfilPorApplicationUserIdAsync(userIdClaim);
        }
        // GET: /Details/HorariosOcupados?data=2026-05-10
        [HttpGet]
        public async Task<IActionResult> HorariosOcupados(DateTime data)
        {
            var ocupados = await _agendamentoService.GetHorariosOcupadosNoDiaAsync(data);
            return Json(ocupados);
        }
    }
}