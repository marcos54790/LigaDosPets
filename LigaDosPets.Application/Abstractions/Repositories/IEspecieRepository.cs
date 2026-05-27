using LigaDosPets.Domain.Entities;

namespace LigaDosPets.Application.Abstractions.Repositories
{
    public interface IEspecieRepository : IRepository<Especie>
    {
        Task<Especie?> GetEspecieComRacasAsync(int id);
        Task<bool> ExistePorNomeAsync(string nome);
    }
}