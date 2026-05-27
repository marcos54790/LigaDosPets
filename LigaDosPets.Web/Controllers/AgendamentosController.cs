// Web/Controllers/AgendamentosController.cs
using LigaDosPets.Application.Abstractions.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

[Authorize]
public class AgendamentosController : Controller
{
    private readonly IAgendamentoService _agendamentoService;
    private readonly IUsuarioService _usuarioService;

    public AgendamentosController(IAgendamentoService agendamentoService, IUsuarioService usuarioService)
    {
        _agendamentoService = agendamentoService;
        _usuarioService = usuarioService;
    }

    private async Task<int?> GetUsuarioId()
    {
        var claim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(claim)) return null;
        var perfil = await _usuarioService.GetPerfilPorApplicationUserIdAsync(claim);
        return perfil?.Id;
    }

    // GET: /Agendamentos/MeusAgendamentos
    public async Task<IActionResult> MeusAgendamentos()
    {
        var usuarioId = await GetUsuarioId();
        if (usuarioId == null) return RedirectToAction("Login", "Auth");

        var agendamentos = await _agendamentoService.GetAgendamentosUsuarioAsync(usuarioId.Value);
        return View(agendamentos.OrderByDescending(a => a.DataVisita));
    }

    // POST: /Agendamentos/Cancelar/{id}
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Cancelar(int id, string motivo)
    {
        var usuarioId = await GetUsuarioId();
        if (usuarioId == null) return RedirectToAction("Login", "Auth");

        try
        {
            // Validar que o agendamento pertence ao usuário
            var ag = await _agendamentoService.GetByIdAsync(id);
            if (ag == null || ag.UsuarioId != usuarioId)
            {
                TempData["Erro"] = "Agendamento não encontrado.";
                return RedirectToAction(nameof(MeusAgendamentos));
            }

            await _agendamentoService.CancelarAgendamentoAsync(
                id, motivo ?? "Cancelado pelo usuário.", usuarioId.Value);
            TempData["Sucesso"] = "Agendamento cancelado com sucesso.";
        }
        catch (Exception ex)
        {
            TempData["Erro"] = ex.Message;
        }

        return RedirectToAction(nameof(MeusAgendamentos));
    }
}