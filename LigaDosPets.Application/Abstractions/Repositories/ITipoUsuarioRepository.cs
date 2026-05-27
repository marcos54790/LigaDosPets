using LigaDosPets.Domain.Entities;

namespace LigaDosPets.Application.Abstractions.Repositories
{
    public interface ITipoUsuarioRepository : IRepository<TipoUsuario>
    {
        Task<TipoUsuario?> ObterPorNomeAsync(string nome);
    }
}