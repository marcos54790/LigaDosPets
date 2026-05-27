using LigaDosPets.Application.DTOs;
using LigaDosPets.Application.DTOs.Porte;

namespace LigaDosPets.Application.Abstractions.Services
{
    public interface IPorteService
    {
        Task<IEnumerable<PorteDto>> GetAllAsync();
        Task<PorteDto?> GetByIdAsync(int id);
        Task<PorteDto> CreateAsync(CreatePorteDto dto);
        Task UpdateAsync(int id, UpdatePorteDto dto);
        Task DeleteAsync(int id);
    }
}