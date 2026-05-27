using LigaDosPets.Application.DTOs.Caracteristica;

namespace LigaDosPets.Application.Abstractions.Services
{
    public interface ICaracteristicaService
    {
        Task<IEnumerable<CaracteristicaDto>> GetAllAsync();
        Task<CaracteristicaDto?> GetByIdAsync(int id);
        Task<CaracteristicaDto> CreateAsync(CreateCaracteristicaDto dto);
        Task UpdateAsync(int id, UpdateCaracteristicaDto dto);
        Task DeleteAsync(int id);
    }
}