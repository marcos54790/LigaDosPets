using LigaDosPets.Domain.Entities;

namespace LigaDosPets.Application.Abstractions.Repositories
{
    public interface IFotoRepository : IRepository<Foto>
    {
        Task<IEnumerable<Foto>> GetFotosPorAnimalAsync(int animalId);
        Task<Foto?> GetFotoPrincipalAsync(int animalId);
        Task DefinirFotoPrincipalAsync(int fotoId);
    }
}