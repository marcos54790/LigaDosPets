using LigaDosPets.Application.Abstractions;
using LigaDosPets.Application.Abstractions.Repositories;
using LigaDosPets.Application.Abstractions.Services;
using LigaDosPets.Application.DTOs.Caracteristica;
using LigaDosPets.Domain.Entities;

namespace LigaDosPets.Application.Services
{
    public class CaracteristicaService : ICaracteristicaService
    {
        private readonly ICaracteristicaRepository _repository;
        private readonly IUnitOfWork _unitOfWork;

        public CaracteristicaService(ICaracteristicaRepository repository, IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        private static CaracteristicaDto MapToDto(Caracteristica c) => new() { Id = c.Id, Nome = c.Nome };

        public async Task<IEnumerable<CaracteristicaDto>> GetAllAsync()
        {
            var list = await _repository.GetAllAsync();
            return list.Select(MapToDto);
        }

        public async Task<CaracteristicaDto?> GetByIdAsync(int id)
        {
            var c = await _repository.GetByIdAsync(id);
            return c == null ? null : MapToDto(c);
        }

        public async Task<CaracteristicaDto> CreateAsync(CreateCaracteristicaDto dto)
        {
            var caracteristica = new Caracteristica { Nome = dto.Nome };
            await _repository.AddAsync(caracteristica);
            await _unitOfWork.SaveChangesAsync();
            return MapToDto(caracteristica);
        }

        public async Task UpdateAsync(int id, UpdateCaracteristicaDto dto)
        {
            var c = await _repository.GetByIdAsync(id);
            if (c == null) throw new InvalidOperationException("Característica não encontrada.");
            c.Nome = dto.Nome;
            await _repository.UpdateAsync(c);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var c = await _repository.GetByIdAsync(id);
            if (c == null) throw new InvalidOperationException("Característica não encontrada.");
            await _repository.DeleteAsync(c);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}