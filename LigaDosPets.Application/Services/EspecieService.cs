using LigaDosPets.Application.Abstractions;
using LigaDosPets.Application.Abstractions.Repositories;
using LigaDosPets.Application.Abstractions.Services;
using LigaDosPets.Application.DTOs.Especie;
using LigaDosPets.Application.DTOs.Raca;
using LigaDosPets.Domain.Entities;

namespace LigaDosPets.Application.Services
{
    public class EspecieService : IEspecieService
    {
        private readonly IEspecieRepository _especieRepository;
        private readonly IRacaRepository _racaRepository;
        private readonly IUnitOfWork _unitOfWork;

        public EspecieService(IEspecieRepository especieRepository, IRacaRepository racaRepository, IUnitOfWork unitOfWork)
        {
            _especieRepository = especieRepository;
            _racaRepository = racaRepository;
            _unitOfWork = unitOfWork;
        }

        private static EspecieDto MapToDto(Especie e) => new() { Id = e.Id, Nome = e.Nome };

        public async Task<IEnumerable<EspecieDto>> GetAllAsync()
        {
            var list = await _especieRepository.GetAllAsync();
            return list.Select(MapToDto);
        }

        public async Task<EspecieDto?> GetByIdAsync(int id)
        {
            var e = await _especieRepository.GetByIdAsync(id);
            return e == null ? null : MapToDto(e);
        }

        public async Task<EspecieDto> CreateAsync(CreateEspecieDto dto)
        {
            var especie = new Especie { Nome = dto.Nome };
            await _especieRepository.AddAsync(especie);
            await _unitOfWork.SaveChangesAsync();
            return MapToDto(especie);
        }

        public async Task UpdateAsync(int id, UpdateEspecieDto dto)
        {
            var e = await _especieRepository.GetByIdAsync(id);
            if (e == null) throw new InvalidOperationException("Espécie não encontrada.");
            e.Nome = dto.Nome;
            await _especieRepository.UpdateAsync(e);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var e = await _especieRepository.GetByIdAsync(id);
            if (e == null) throw new InvalidOperationException("Espécie não encontrada.");
            await _especieRepository.DeleteAsync(e);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<IEnumerable<RacaDto>> GetRacasPorEspecieAsync(int especieId)
        {
            var racas = await _racaRepository.GetRacasPorEspecieAsync(especieId);
            return racas.Select(r => new RacaDto { Id = r.Id, Nome = r.Nome, EspecieId = r.EspecieId, NomeEspecie = r.Especie?.Nome ?? string.Empty });
        }
    }
}