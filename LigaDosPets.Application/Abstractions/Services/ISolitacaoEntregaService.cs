using LigaDosPets.Application.DTOs.SolicitacaoEntrega;

namespace LigaDosPets.Application.Abstractions.Services
{
    public interface ISolicitacaoEntregaAppService
    {
        Task<SolicitacaoDto> CriarSolicitacaoAsync(CreateSolicitacaoDto dto, int usuarioId);
        Task<SolicitacaoDto?> GetByIdAsync(int id);
        Task<IEnumerable<SolicitacaoDto>> GetSolicitacoesUsuarioAsync(int usuarioId);
        Task<IEnumerable<SolicitacaoDto>> GetSolicitacoesPendentesAsync();
        Task<SolicitacaoDto> AprovarSolicitacaoAsync(int id, int adminId, AprovarSolicitacaoDto dto);
        Task<SolicitacaoDto> RejeitarSolicitacaoAsync(int id, int adminId, string motivo);
        Task<SolicitacaoDto> AprovarSolicitacaoSimplesAsync(int id, string? observacaoAdmin);
        Task<SolicitacaoDto> CriarSolicitacaoComFotosAsync(CreateSolicitacaoDto dto, int usuarioId);
    }
}