// Application/Abstractions/Services/IAgendamentoService.cs
using LigaDosPets.Application.DTOs.Agendamento;

namespace LigaDosPets.Application.Abstractions.Services
{
    public interface IAgendamentoService
    {
        Task<AgendamentoDto> CriarAgendamentoAsync(CreateAgendamentoDto dto, int usuarioId);
        Task<AgendamentoDto?> GetByIdAsync(int id);
        Task<IEnumerable<AgendamentoDto>> GetAgendamentosUsuarioAsync(int usuarioId);
        Task<IEnumerable<AgendamentoDto>> GetAgendamentosPendentesAsync();
        Task<IEnumerable<AgendamentoDto>> GetAgendamentosFuturosAsync();
        Task<AgendamentoDto> ConfirmarAgendamentoAsync(int id, int adminId);
        Task<AgendamentoDto> RealizarAgendamentoAsync(int id);
        Task<AgendamentoDto> CancelarAgendamentoAsync(int id, string motivo, int usuarioId);
        Task<IEnumerable<string>> GetHorariosOcupadosNoDiaAsync(DateTime data);
    }
}