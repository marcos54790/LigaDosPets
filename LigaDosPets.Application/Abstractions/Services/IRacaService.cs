using LigaDosPets.Application.DTOs;
using LigaDosPets.Application.DTOs.Raca;

namespace LigaDosPets.Application.Abstractions.Services
{
    public interface IRacaService
    {
        Task<IEnumerable<RacaDto>> GetAllAsync();
        Task<IEnumerable<RacaDto>> GetByEspecieAsync(int especieId);
        Task<RacaDto?> GetByIdAsync(int id);
        Task<RacaDto> CreateAsync(CreateRacaDto dto);
        Task UpdateAsync(int id, UpdateRacaDto dto);
        Task DeleteAsync(int id);
    }
}