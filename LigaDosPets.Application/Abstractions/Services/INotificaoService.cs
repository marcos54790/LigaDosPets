using LigaDosPets.Application.DTOs.Notificacao;

namespace LigaDosPets.Application.Abstractions.Services
{
    public interface INotificacaoService
    {
        Task<IEnumerable<NotificacaoDto>> GetNotificacoesUsuarioAsync(int usuarioId);
        Task<NotificacaoDto?> MarcarComoLidaAsync(int notificacaoId);
        Task MarcarTodasComoLidasAsync(int usuarioId);
        Task<int> ContarNaoLidasAsync(int usuarioId);
        Task NotificarAsync(int usuarioId, string titulo, string mensagem, string? urlAcao);  // ← NOVO
    }
}