using LigaDosPets.Domain.Entities;

namespace LigaDosPets.Application.Abstractions.Repositories
{
    public interface ISolicitacaoEntregaRepository : IRepository<SolicitacaoEntrega>
    {
        Task<IEnumerable<SolicitacaoEntrega>> GetSolicitacoesPendentesAsync();
        Task<IEnumerable<SolicitacaoEntrega>> GetSolicitacoesPorUsuarioAsync(int usuarioId);
        Task<IEnumerable<SolicitacaoEntrega>> GetSolicitacoesPorStatusAsync(int statusId);
        Task<SolicitacaoEntrega?> GetSolicitacaoCompletaAsync(int id);
    }
}