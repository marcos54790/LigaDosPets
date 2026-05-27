using LigaDosPets.Domain.Entities;

namespace LigaDosPets.Application.Abstractions.Repositories
{
    public interface IMensagemRepository : IRepository<Mensagem>
    {
        Task<IEnumerable<Mensagem>> GetMensagensPorConversaAsync(int conversaId);
        Task<int> ContarMensagensNaoLidasAsync(int conversaId, int usuarioId);
        Task MarcarComoLidasAsync(int conversaId, int usuarioId);
    }
}