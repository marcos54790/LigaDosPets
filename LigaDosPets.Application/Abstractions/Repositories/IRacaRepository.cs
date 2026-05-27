using LigaDosPets.Domain.Entities;

namespace LigaDosPets.Application.Abstractions.Repositories
{
    public interface IRacaRepository : IRepository<Raca>
    {
        Task<IEnumerable<Raca>> GetRacasPorEspecieAsync(int especieId);
        Task<bool> ExistePorNomeEEspecieAsync(string nome, int especieId);
    }
}