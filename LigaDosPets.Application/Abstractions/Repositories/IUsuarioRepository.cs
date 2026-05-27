using LigaDosPets.Domain.Entities;

namespace LigaDosPets.Application.Abstractions.Repositories
{
    public interface IUsuarioRepository : IRepository<Usuario>
    {
        Task<Usuario?> ObterPorEmailAsync(string email);
        Task<Usuario?> ObterPorApplicationUserIdAsync(string applicationUserId);
        Task<Usuario?> ObterUsuarioCompletoAsync(int id);
        Task<IEnumerable<Usuario>> ObterAdministradoresAsync();
        Task<bool> EmailExisteAsync(string email);
    }
}