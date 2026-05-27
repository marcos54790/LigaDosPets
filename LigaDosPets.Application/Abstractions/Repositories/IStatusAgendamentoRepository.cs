using LigaDosPets.Domain.Entities;

namespace LigaDosPets.Application.Abstractions.Repositories
{
    public interface IStatusAgendamentoRepository : IRepository<StatusAgendamento>
    {
        Task<StatusAgendamento?> ObterPorNomeAsync(string nome);
    }
}