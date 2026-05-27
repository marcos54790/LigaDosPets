using LigaDosPets.Domain.Entities;

namespace LigaDosPets.Application.Abstractions.Repositories
{
    public interface INotificacaoRepository : IRepository<Notificacao>
    {
        Task<IEnumerable<Notificacao>> GetNotificacoesPorUsuarioAsync(int usuarioId);
        Task<IEnumerable<Notificacao>> GetNotificacoesNaoLidasAsync(int usuarioId);
        Task<int> ContarNaoLidasAsync(int usuarioId);
        Task MarcarComoLidaAsync(int id);
        Task MarcarTodasComoLidasAsync(int usuarioId);
    }
}