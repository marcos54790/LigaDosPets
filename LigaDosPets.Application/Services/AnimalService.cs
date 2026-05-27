using LigaDosPets.Application.Abstractions;
using LigaDosPets.Application.Abstractions.Repositories;
using LigaDosPets.Application.Abstractions.Services;
using LigaDosPets.Application.DTOs.Animal;
using LigaDosPets.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LigaDosPets.Application.Services
{
    public class AnimalService : IAnimalService
    {
        private readonly IAnimalRepository _animalRepository;
        private readonly IPorteRepository _porteRepository;
        private readonly IEspecieRepository _especieRepository;
        private readonly IRacaRepository _racaRepository;
        private readonly ICaracteristicaRepository _caracteristicaRepository;
        private readonly IFotoService _fotoService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly INotificacaoService _notificacaoService;

        public AnimalService(
            IAnimalRepository animalRepository,
            IPorteRepository porteRepository,
            IEspecieRepository especieRepository,
            IRacaRepository racaRepository,
            ICaracteristicaRepository caracteristicaRepository,
            IFotoService fotoService,
            IUnitOfWork unitOfWork,
            INotificacaoService notificacaoService)
        {
            _animalRepository = animalRepository;
            _porteRepository = porteRepository;
            _especieRepository = especieRepository;
            _racaRepository = racaRepository;
            _caracteristicaRepository = caracteristicaRepository;
            _fotoService = fotoService;
            _unitOfWork = unitOfWork;
            _notificacaoService = notificacaoService;
        }

        // ── Mappers ────────────────────────────────────────────────────────────
        private static AnimalListDto MapToListDto(Animal a) => new()
        {
            Id = a.Id,
            Nome = a.Nome,
            Idade = a.Idade,
            Sexo = a.Sexo,
            Porte = a.Porte?.Nome ?? string.Empty,
            Especie = a.Especie?.Nome ?? string.Empty,
            Raca = a.Raca?.Nome,
            FotoPrincipalUrl = a.Fotos?.FirstOrDefault(f => f.IsPrincipal)?.Url
                                   ?? a.Fotos?.FirstOrDefault()?.Url,
            DisponivelParaAdocao = a.DisponivelParaAdocao,
            AguardandoValidacao = a.AguardandoValidacao
        };

        private static AnimalDetailDto MapToDetailDto(Animal a) => new()
        {
            Id = a.Id,
            Nome = a.Nome,
            Idade = a.Idade,
            Sexo = a.Sexo,
            Porte = a.Porte?.Nome ?? string.Empty,
            Especie = a.Especie?.Nome ?? string.Empty,
            Raca = a.Raca?.Nome,
            FotoPrincipalUrl = a.Fotos?.FirstOrDefault(f => f.IsPrincipal)?.Url
                                   ?? a.Fotos?.FirstOrDefault()?.Url,
            DisponivelParaAdocao = a.DisponivelParaAdocao,
            AguardandoValidacao = a.AguardandoValidacao,
            Descricao = a.Descricao,
            MotivoEntrada = a.MotivoEntrada,
            Fotos = a.Fotos?.Select(f => f.Url).ToList() ?? new(),
            Caracteristicas = a.AnimalCaracteristicas?
                                    .Select(ac => ac.Caracteristica.Nome).ToList() ?? new(),
            NomeUsuarioCadastro = a.UsuarioCadastro?.Nome ?? string.Empty,
            UsuarioCadastroId = a.UsuarioCadastroId,
            DataCadastro = a.DataCadastro
        };

        // ── Helper: busca foto principal por ids ───────────────────────────────
        private async Task<Dictionary<int, string?>> GetFotosPrincipaisAsync(List<int> ids)
        {
            var fotos = await _animalRepository.Query()
                .Where(a => ids.Contains(a.Id))
                .Select(a => new
                {
                    a.Id,
                    FotoUrl = a.Fotos
                        .Where(f => f.IsPrincipal)
                        .Select(f => f.Url)
                        .FirstOrDefault()
                        ?? a.Fotos
                        .Select(f => f.Url)
                        .FirstOrDefault()
                })
                .ToListAsync();

            return fotos.ToDictionary(f => f.Id, f => f.FotoUrl);
        }

        // ── Catálogo público ───────────────────────────────────────────────────
        public async Task<IEnumerable<AnimalListDto>> GetAnimaisDisponiveisAsync(AnimalFiltroDto filtro)
        {
            // Busca animais SEM Include(Fotos) para evitar o JOIN que duplica linhas.
            // A foto principal é buscada em query separada via GetFotosPrincipaisAsync.
            var query = _animalRepository.Query()
                .Include(a => a.Porte)
                .Include(a => a.Especie)
                .Include(a => a.Raca)
                .Where(a => a.DisponivelParaAdocao && !a.AguardandoValidacao);

            if (filtro.EspecieId.HasValue)
                query = query.Where(a => a.EspecieId == filtro.EspecieId.Value);
            if (filtro.PorteId.HasValue)
                query = query.Where(a => a.PorteId == filtro.PorteId.Value);
            if (filtro.RacaId.HasValue)
                query = query.Where(a => a.RacaId == filtro.RacaId.Value);
            if (!string.IsNullOrEmpty(filtro.Sexo))
                query = query.Where(a => a.Sexo == filtro.Sexo);
            if (filtro.IdadeMinima.HasValue)
                query = query.Where(a => a.Idade >= filtro.IdadeMinima.Value);
            if (filtro.IdadeMaxima.HasValue)
                query = query.Where(a => a.Idade <= filtro.IdadeMaxima.Value);

            var animais = await query.ToListAsync();
            var ids = animais.Select(a => a.Id).ToList();
            var fotosDict = await GetFotosPrincipaisAsync(ids);

            return animais.Select(a =>
            {
                var dto = MapToListDto(a);
                dto.FotoPrincipalUrl = fotosDict.GetValueOrDefault(a.Id);
                return dto;
            });
        }

        // ── Admin: todos os animais ────────────────────────────────────────────
        public async Task<IEnumerable<AnimalListDto>> GetTodosAnimaisAsync()
        {
            var animais = await _animalRepository.Query()
                .Include(a => a.Porte)
                .Include(a => a.Especie)
                .Include(a => a.Raca)
                .OrderByDescending(a => a.DataCadastro)
                .ToListAsync();

            var ids = animais.Select(a => a.Id).ToList();
            var fotosDict = await GetFotosPrincipaisAsync(ids);

            return animais.Select(a =>
            {
                var dto = MapToListDto(a);
                dto.FotoPrincipalUrl = fotosDict.GetValueOrDefault(a.Id);
                return dto;
            });
        }

        // ── Admin: pendentes de validação ──────────────────────────────────────
        public async Task<IEnumerable<AnimalListDto>> GetAnimaisPendentesValidacaoAsync()
        {
            var animais = await _animalRepository.GetPendentesValidacaoAsync();
            var ids = animais.Select(a => a.Id).ToList();
            var fotosDict = await GetFotosPrincipaisAsync(ids);

            return animais.Select(a =>
            {
                var dto = MapToListDto(a);
                dto.FotoPrincipalUrl = fotosDict.GetValueOrDefault(a.Id);
                return dto;
            });
        }

        // ── Animais por usuário ────────────────────────────────────────────────
        public async Task<IEnumerable<AnimalListDto>> GetAnimaisPorUsuarioAsync(int usuarioId)
        {
            var animais = await _animalRepository.GetAnimaisPorUsuarioAsync(usuarioId);
            var ids = animais.Select(a => a.Id).ToList();
            var fotosDict = await GetFotosPrincipaisAsync(ids);

            return animais.Select(a =>
            {
                var dto = MapToListDto(a);
                dto.FotoPrincipalUrl = fotosDict.GetValueOrDefault(a.Id);
                return dto;
            });
        }

        // ── Detalhe ────────────────────────────────────────────────────────────
        public async Task<AnimalDetailDto?> GetByIdAsync(int id)
        {
            var animal = await _animalRepository.GetAnimalComDetalhesAsync(id);
            return animal == null ? null : MapToDetailDto(animal);
        }

        // ── Criar ──────────────────────────────────────────────────────────────
        public async Task<AnimalDetailDto> CreateAsync(
            CreateAnimalDto dto, int usuarioCadastroId, bool isAdmin = false)
        {
            if (!await _porteRepository.AnyAsync(p => p.Id == dto.PorteId))
                throw new InvalidOperationException("Porte inválido.");
            if (!await _especieRepository.AnyAsync(e => e.Id == dto.EspecieId))
                throw new InvalidOperationException("Espécie inválida.");
            if (dto.RacaId.HasValue &&
                !await _racaRepository.AnyAsync(
                    r => r.Id == dto.RacaId.Value && r.EspecieId == dto.EspecieId))
                throw new InvalidOperationException("Raça inválida para a espécie selecionada.");

            var animal = new Animal
            {
                Nome = dto.Nome,
                Idade = dto.Idade,
                Sexo = dto.Sexo,
                Descricao = dto.Descricao,
                MotivoEntrada = dto.MotivoDoacao,
                PorteId = dto.PorteId,
                EspecieId = dto.EspecieId,
                RacaId = dto.RacaId,
                UsuarioCadastroId = usuarioCadastroId,
                DisponivelParaAdocao = true,
                AguardandoValidacao = !isAdmin
            };

            foreach (var caracteristicaId in dto.CaracteristicasIds)
            {
                if (await _caracteristicaRepository.AnyAsync(c => c.Id == caracteristicaId))
                    animal.AnimalCaracteristicas.Add(
                        new AnimalCaracteristica { CaracteristicaId = caracteristicaId });
            }

            await _animalRepository.AddAsync(animal);
            await _unitOfWork.SaveChangesAsync();

            if (dto.Fotos != null && dto.Fotos.Any())
            {
                bool primeira = true;
                foreach (var foto in dto.Fotos)
                {
                    await _fotoService.UploadFotoAsync(animal.Id, foto, isPrincipal: primeira);
                    primeira = false;
                }
            }

            var completo = await _animalRepository.GetAnimalComDetalhesAsync(animal.Id)
                           ?? throw new InvalidOperationException("Erro ao recuperar o animal criado.");
            return MapToDetailDto(completo);
        }

        // ── Editar ─────────────────────────────────────────────────────────────
        public async Task<AnimalDetailDto> UpdateAsync(int id, UpdateAnimalDto dto)
        {
            var animal = await _animalRepository.GetAnimalComDetalhesAsync(id)
                         ?? throw new InvalidOperationException("Animal não encontrado.");

            animal.Nome = dto.Nome;
            animal.Idade = dto.Idade;
            animal.Sexo = dto.Sexo;
            animal.Descricao = dto.Descricao;
            animal.PorteId = dto.PorteId;
            animal.EspecieId = dto.EspecieId;
            animal.RacaId = dto.RacaId;

            animal.AnimalCaracteristicas.Clear();
            foreach (var caracteristicaId in dto.CaracteristicasIds)
            {
                if (await _caracteristicaRepository.AnyAsync(c => c.Id == caracteristicaId))
                    animal.AnimalCaracteristicas.Add(
                        new AnimalCaracteristica { CaracteristicaId = caracteristicaId });
            }

            await _animalRepository.UpdateAsync(animal);
            await _unitOfWork.SaveChangesAsync();
            return MapToDetailDto(animal);
        }

        // ── Excluir ────────────────────────────────────────────────────────────
        public async Task DeleteAsync(int id)
        {
            var animal = await _animalRepository.GetByIdAsync(id)
                         ?? throw new InvalidOperationException("Animal não encontrado.");
            await _animalRepository.DeleteAsync(animal);
            await _unitOfWork.SaveChangesAsync();
        }

        // ── Validação / Rejeição ───────────────────────────────────────────────
        public async Task ValidarAnimalAsync(int id)
        {
            var animal = await _animalRepository.GetByIdAsync(id)
                         ?? throw new InvalidOperationException("Animal não encontrado.");

            animal.AguardandoValidacao = false;
            await _animalRepository.UpdateAsync(animal);
            await _unitOfWork.SaveChangesAsync();

            await _notificacaoService.NotificarAsync(
                animal.UsuarioCadastroId,
                "Animal aprovado!",
                $"Seu pet '{animal.Nome}' foi aprovado e agora está no catálogo.",
                "/Details/Details/" + animal.Id);
        }

        public async Task RejeitarAnimalAsync(int id, string motivo)
        {
            var animal = await _animalRepository.GetByIdAsync(id)
                         ?? throw new InvalidOperationException("Animal não encontrado.");

            int usuarioId = animal.UsuarioCadastroId;
            string nomeAnimal = animal.Nome;

            await _animalRepository.DeleteAsync(animal);
            await _unitOfWork.SaveChangesAsync();

            await _notificacaoService.NotificarAsync(
                usuarioId,
                "Animal recusado",
                $"Seu pet '{nomeAnimal}' foi recusado. Motivo: {motivo}",
                null);
        }

        // ── Adoção ─────────────────────────────────────────────────────────────
        public async Task MarcarComoAdotadoAsync(int animalId)
        {
            var animal = await _animalRepository.GetByIdAsync(animalId)
                         ?? throw new InvalidOperationException("Animal não encontrado.");

            animal.DisponivelParaAdocao = false;
            await _animalRepository.UpdateAsync(animal);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}