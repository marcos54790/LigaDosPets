using LigaDosPets.Application.Abstractions;
using LigaDosPets.Application.Abstractions.Repositories;
using LigaDosPets.Application.Abstractions.Services;
using LigaDosPets.Application.DTOs.Raca;
using LigaDosPets.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LigaDosPets.Application.Services
{
    public class RacaService : IRacaService
    {
        private readonly IRacaRepository _racaRepository;
        private readonly IEspecieRepository _especieRepository;
        private readonly IUnitOfWork _unitOfWork;

        public RacaService(
            IRacaRepository racaRepository,
            IEspecieRepository especieRepository,
            IUnitOfWork unitOfWork)
        {
            _racaRepository = racaRepository;
            _especieRepository = especieRepository;
            _unitOfWork = unitOfWork;
        }

        private static RacaDto MapToDto(Raca r) => new()
        {
            Id = r.Id,
            Nome = r.Nome,
            EspecieId = r.EspecieId,
            NomeEspecie = r.Especie?.Nome ?? string.Empty
        };

        public async Task<IEnumerable<RacaDto>> GetAllAsync()
        {
            var list = await _racaRepository.Query()
                .Include(r => r.Especie)
                .ToListAsync();
            return list.Select(MapToDto);
        }

        public async Task<IEnumerable<RacaDto>> GetByEspecieAsync(int especieId)
        {
            var list = await _racaRepository.GetRacasPorEspecieAsync(especieId);
            return list.Select(MapToDto);
        }

        public async Task<RacaDto?> GetByIdAsync(int id)
        {
            var r = await _racaRepository.Query()
                .Include(r => r.Especie)
                .FirstOrDefaultAsync(r => r.Id == id);
            return r == null ? null : MapToDto(r);
        }

        public async Task<RacaDto> CreateAsync(CreateRacaDto dto)
        {
            if (!await _especieRepository.AnyAsync(e => e.Id == dto.EspecieId))
                throw new InvalidOperationException("Espécie inválida.");

            var raca = new Raca
            {
                Nome = dto.Nome,
                EspecieId = dto.EspecieId
            };

            await _racaRepository.AddAsync(raca);
            await _unitOfWork.SaveChangesAsync();

            // Carregar espécie para o DTO
            raca.Especie = await _especieRepository.GetByIdAsync(dto.EspecieId);
            return MapToDto(raca);
        }

        public async Task UpdateAsync(int id, UpdateRacaDto dto)
        {
            var r = await _racaRepository.GetByIdAsync(id);
            if (r == null) throw new InvalidOperationException("Raça não encontrada.");
            r.Nome = dto.Nome;
            await _racaRepository.UpdateAsync(r);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var r = await _racaRepository.GetByIdAsync(id);
            if (r == null) throw new InvalidOperationException("Raça não encontrada.");
            await _racaRepository.DeleteAsync(r);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}