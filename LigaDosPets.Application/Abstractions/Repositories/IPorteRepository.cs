using LigaDosPets.Domain.Entities;

namespace LigaDosPets.Application.Abstractions.Repositories
{
    public interface IPorteRepository : IRepository<Porte>
    {
        Task<bool> ExistePorNomeAsync(string nome);
    }
}