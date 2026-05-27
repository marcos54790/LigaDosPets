using LigaDosPets.Application.Abstractions;
using LigaDosPets.Application.Abstractions.Repositories;
using LigaDosPets.Application.Abstractions.Services;
using LigaDosPets.Application.DTOs.Adocao;
using LigaDosPets.Domain.Entities;

namespace LigaDosPets.Application.Services
{
    public class AdocaoService : IAdocaoService
    {
        private readonly IAdocaoRepository _adocaoRepository;
        private readonly IAnimalRepository _animalRepository;
        private readonly IUnitOfWork _unitOfWork;

        public AdocaoService(IAdocaoRepository adocaoRepository, IAnimalRepository animalRepository, IUnitOfWork unitOfWork)
        {
            _adocaoRepository = adocaoRepository;
            _animalRepository = animalRepository;
            _unitOfWork = unitOfWork;
        }

        private static AdocaoDto MapToDto(Adocao adocao)
        {
            return new AdocaoDto
            {
                Id = adocao.Id,
                AnimalId = adocao.AnimalId,
                NomeAnimal = adocao.Animal?.Nome ?? string.Empty,
                UsuarioAdotanteId = adocao.UsuarioAdotanteId,
                NomeAdotante = adocao.UsuarioAdotante?.Nome ?? string.Empty,
                AdminConfirmouId = adocao.AdminConfirmouId,
                NomeAdmin = adocao.AdminConfirmou?.Nome ?? string.Empty,
                DataAdocao = adocao.DataAdocao,
                TermosAssinados = adocao.TermosAssinados,
                Observacao = adocao.Observacao
            };
        }

        public async Task<bool> AnimalPodeSerAdotadoAsync(int animalId)
        {
            var animal = await _animalRepository.GetByIdAsync(animalId);
            return animal != null && animal.DisponivelParaAdocao && !animal.AguardandoValidacao;
        }

        public async Task<AdocaoDto?> GetByIdAsync(int id)
        {
            var adocao = await _adocaoRepository.GetAdocaoComRelacionamentosAsync(id);
            return adocao == null ? null : MapToDto(adocao);
        }

        public async Task<IEnumerable<AdocaoDto>> GetAdocoesPorAdotanteAsync(int usuarioId)
        {
            var adocoes = await _adocaoRepository.GetAdocoesPorAdotanteAsync(usuarioId);
            return adocoes.Select(MapToDto);
        }

        public async Task<IEnumerable<AdocaoDto>> GetHistoricoAsync()
        {
            var adocoes = await _adocaoRepository.GetAllAsync();
            // Para cada adoção, precisamos carregar os relacionamentos.
            // Vamos obter com relacionamentos via repositório customizado.
            var adocoesCompletas = new List<Adocao>();
            foreach (var a in adocoes)
            {
                var completa = await _adocaoRepository.GetAdocaoComRelacionamentosAsync(a.Id);
                if (completa != null) adocoesCompletas.Add(completa);
            }
            return adocoesCompletas.Select(MapToDto);
        }

        public async Task<AdocaoDto> RegistrarAdocaoAsync(RegistrarAdocaoDto dto, int adminId)
        {
            if (!await AnimalPodeSerAdotadoAsync(dto.AnimalId))
                throw new InvalidOperationException("Animal não está disponível para adoção.");

            var adocao = new Adocao
            {
                AnimalId = dto.AnimalId,
                UsuarioAdotanteId = dto.UsuarioAdotanteId,
                AdminConfirmouId = adminId,
                DataAdocao = DateTime.UtcNow,
                TermosAssinados = dto.TermosAssinados,
                Observacao = dto.Observacao
            };

            await _adocaoRepository.AddAsync(adocao);

            var animal = await _animalRepository.GetByIdAsync(dto.AnimalId);
            animal!.DisponivelParaAdocao = false;
            await _animalRepository.UpdateAsync(animal);

            await _unitOfWork.SaveChangesAsync();

            var adocaoCompleta = await _adocaoRepository.GetAdocaoComRelacionamentosAsync(adocao.Id);
            return MapToDto(adocaoCompleta!);
        }
    }
}