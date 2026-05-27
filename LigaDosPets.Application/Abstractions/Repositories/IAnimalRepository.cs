using System.Linq.Expressions;
using LigaDosPets.Domain.Entities;

namespace LigaDosPets.Application.Abstractions.Repositories
{
    public interface IAnimalRepository : IRepository<Animal>
    {
        Task<IEnumerable<Animal>> GetDisponiveisParaAdocaoAsync();
        Task<IEnumerable<Animal>> GetPendentesValidacaoAsync();
        Task<Animal?> GetAnimalComDetalhesAsync(int id);
        Task<IEnumerable<Animal>> GetAnimaisPorUsuarioAsync(int usuarioId);

        // Adicionado para resolver o erro CS1061 no AnimalService
        Task<IEnumerable<Animal>> GetByExpressionAsync(Expression<Func<Animal, bool>> expression);
    }
}