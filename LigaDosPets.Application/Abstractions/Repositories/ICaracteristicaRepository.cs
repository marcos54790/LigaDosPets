using LigaDosPets.Domain.Entities;

namespace LigaDosPets.Application.Abstractions.Repositories
{
    public interface ICaracteristicaRepository : IRepository<Caracteristica>
    {
        Task<IEnumerable<Caracteristica>> GetCaracteristicasPorAnimalAsync(int animalId);
        Task<bool> ExistePorNomeAsync(string nome);
    }
}