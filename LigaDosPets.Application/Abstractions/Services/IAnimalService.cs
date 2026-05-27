using LigaDosPets.Application.DTOs.Animal;

namespace LigaDosPets.Application.Abstractions.Services
{
    public interface IAnimalService
    {
        Task<AnimalDetailDto?> GetByIdAsync(int id);
        Task<IEnumerable<AnimalListDto>> GetAnimaisDisponiveisAsync(AnimalFiltroDto filtro);
        Task<IEnumerable<AnimalListDto>> GetAnimaisPendentesValidacaoAsync();
        Task<AnimalDetailDto> CreateAsync(CreateAnimalDto dto, int usuarioCadastroId, bool isAdmin = false);
        Task<AnimalDetailDto> UpdateAsync(int id, UpdateAnimalDto dto);
        Task DeleteAsync(int id);
        Task ValidarAnimalAsync(int id);
        Task RejeitarAnimalAsync(int id, string motivo);
        Task<IEnumerable<AnimalListDto>> GetAnimaisPorUsuarioAsync(int usuarioId);
        Task MarcarComoAdotadoAsync(int animalId);

    }
}