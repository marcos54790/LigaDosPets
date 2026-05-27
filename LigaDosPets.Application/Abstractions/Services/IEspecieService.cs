using LigaDosPets.Application.DTOs.Especie;
using LigaDosPets.Application.DTOs.Raca;

namespace LigaDosPets.Application.Abstractions.Services
{
    public interface IEspecieService
    {
        Task<IEnumerable<EspecieDto>> GetAllAsync();
        Task<EspecieDto?> GetByIdAsync(int id);
        Task<EspecieDto> CreateAsync(CreateEspecieDto dto);
        Task UpdateAsync(int id, UpdateEspecieDto dto);
        Task DeleteAsync(int id);
        Task<IEnumerable<RacaDto>> GetRacasPorEspecieAsync(int especieId);
    }
}