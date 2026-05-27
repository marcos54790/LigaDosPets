using LigaDosPets.Domain.Entities;

namespace LigaDosPets.Application.Abstractions.Repositories
{
    public interface IAdocaoRepository : IRepository<Adocao>
    {
        Task<Adocao?> GetAdocaoComRelacionamentosAsync(int id);
        Task<IEnumerable<Adocao>> GetAdocoesPorAnimalAsync(int animalId);
        Task<IEnumerable<Adocao>> GetAdocoesPorAdotanteAsync(int usuarioId);
        Task<bool> AnimalJaFoiAdotadoAsync(int animalId);
    }
}