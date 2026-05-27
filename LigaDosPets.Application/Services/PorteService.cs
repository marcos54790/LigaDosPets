using LigaDosPets.Application.Abstractions;
using LigaDosPets.Application.Abstractions.Repositories;
using LigaDosPets.Application.Abstractions.Services;
using LigaDosPets.Application.DTOs.Porte;
using LigaDosPets.Domain.Entities;

namespace LigaDosPets.Application.Services
{
    public class PorteService : IPorteService
    {
        private readonly IPorteRepository _porteRepository;
        private readonly IUnitOfWork _unitOfWork;

        public PorteService(IPorteRepository porteRepository, IUnitOfWork unitOfWork)
        {
            _porteRepository = porteRepository;
            _unitOfWork = unitOfWork;
        }

        private static PorteDto MapToDto(Porte p) => new()
        {
            Id = p.Id,
            Nome = p.Nome
        };

        public async Task<IEnumerable<PorteDto>> GetAllAsync()
        {
            var list = await _porteRepository.GetAllAsync();
            return list.Select(MapToDto);
        }

        public async Task<PorteDto?> GetByIdAsync(int id)
        {
            var p = await _porteRepository.GetByIdAsync(id);
            return p == null ? null : MapToDto(p);
        }

        public async Task<PorteDto> CreateAsync(CreatePorteDto dto)
        {
            var porte = new Porte { Nome = dto.Nome };
            await _porteRepository.AddAsync(porte);
            await _unitOfWork.SaveChangesAsync();
            return MapToDto(porte);
        }

        public async Task UpdateAsync(int id, UpdatePorteDto dto)
        {
            var p = await _porteRepository.GetByIdAsync(id);
            if (p == null) throw new InvalidOperationException("Porte não encontrado.");
            p.Nome = dto.Nome;
            await _porteRepository.UpdateAsync(p);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var p = await _porteRepository.GetByIdAsync(id);
            if (p == null) throw new InvalidOperationException("Porte não encontrado.");
            await _porteRepository.DeleteAsync(p);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}