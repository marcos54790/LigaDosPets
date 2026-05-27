using LigaDosPets.Application.DTOs;
using LigaDosPets.Application.DTOs.Conversa;

namespace LigaDosPets.Application.Abstractions.Services
{
    public interface IConversaService
    {
        Task<ConversaDto> IniciarOuObterConversaAsync(int usuarioLogadoId, int outroUsuarioId, int? animalId);
        Task<MensagemDto> EnviarMensagemAsync(int conversaId, int remetenteId, string conteudo);
        Task<IEnumerable<MensagemDto>> GetMensagensConversaAsync(int conversaId, int usuarioSolicitanteId);
        Task<IEnumerable<ConversaResumoDto>> GetConversasUsuarioAsync(int usuarioId);
        Task MarcarMensagensComoLidasAsync(int conversaId, int usuarioId);
        Task ExcluirConversaAsync(int conversaId);
    }
}