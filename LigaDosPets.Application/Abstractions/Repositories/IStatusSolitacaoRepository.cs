using LigaDosPets.Domain.Entities;

namespace LigaDosPets.Application.Abstractions.Repositories
{
    public interface IStatusSolicitacaoRepository : IRepository<StatusSolicitacao>
    {
        Task<StatusSolicitacao?> ObterPorNomeAsync(string nome);
    }
}