using LigaDosPets.Domain.Entities;

namespace LigaDosPets.Application.Abstractions.Repositories
{
    public interface IConversaRepository : IRepository<Conversa>
    {
        Task<Conversa?> ObterConversaEntreUsuariosAsync(int usuarioAId, int usuarioBId, int? animalId);
        Task<IEnumerable<Conversa>> GetConversasPorUsuarioAsync(int usuarioId);
        Task<Conversa?> GetConversaComMensagensAsync(int conversaId);
        Task AtualizarUltimaMensagemAsync(int conversaId, DateTime data);
    }
}