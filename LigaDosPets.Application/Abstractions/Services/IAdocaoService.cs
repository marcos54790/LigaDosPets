using LigaDosPets.Application.DTOs.Adocao;

namespace LigaDosPets.Application.Abstractions.Services
{
    public interface IAdocaoService
    {
        Task<AdocaoDto> RegistrarAdocaoAsync(RegistrarAdocaoDto dto, int adminId);
        Task<AdocaoDto?> GetByIdAsync(int id);
        Task<IEnumerable<AdocaoDto>> GetHistoricoAsync();
        Task<IEnumerable<AdocaoDto>> GetAdocoesPorAdotanteAsync(int usuarioId);
        Task<bool> AnimalPodeSerAdotadoAsync(int animalId);
    }
}