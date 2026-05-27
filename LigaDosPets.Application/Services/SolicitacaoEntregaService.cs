using LigaDosPets.Application.Abstractions;
using LigaDosPets.Application.Abstractions.Repositories;
using LigaDosPets.Application.Abstractions.Services;
using LigaDosPets.Application.DTOs.Animal;
using LigaDosPets.Application.DTOs.SolicitacaoEntrega;
using LigaDosPets.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using System.Text.RegularExpressions;

namespace LigaDosPets.Application.Services
{
    public class SolicitacaoEntregaService : ISolicitacaoEntregaAppService
    {
        private readonly ISolicitacaoEntregaRepository _solicitacaoRepository;
        private readonly IStatusSolicitacaoRepository _statusRepository;
        private readonly IAnimalService _animalService;
        private readonly IEspecieService _especieService;
        private readonly IPorteService _porteService;
        private readonly INotificacaoService _notificacaoService;
        private readonly ISolicitacaoFotoRepository _solicitacaoFotoRepository;
        private readonly IFotoService _fotoService;
        private readonly IFotoRepository _fotoRepository;
        private readonly IUnitOfWork _unitOfWork;

        public SolicitacaoEntregaService(
            ISolicitacaoEntregaRepository solicitacaoRepository,
            IStatusSolicitacaoRepository statusRepository,
            IAnimalService animalService,
            IEspecieService especieService,
            IPorteService porteService,
            INotificacaoService notificacaoService,
            ISolicitacaoFotoRepository solicitacaoFotoRepository,
            IFotoService fotoService,
            IFotoRepository fotoRepository,
            IUnitOfWork unitOfWork)
        {
            _solicitacaoRepository = solicitacaoRepository;
            _statusRepository = statusRepository;
            _animalService = animalService;
            _especieService = especieService;
            _porteService = porteService;
            _notificacaoService = notificacaoService;
            _solicitacaoFotoRepository = solicitacaoFotoRepository;
            _fotoService = fotoService;
            _fotoRepository = fotoRepository;
            _unitOfWork = unitOfWork;
        }

        private static SolicitacaoDto MapToDto(SolicitacaoEntrega s) => new()
        {
            Id = s.Id,
            UsuarioId = s.UsuarioId,
            NomeUsuario = s.Usuario?.Nome ?? string.Empty,
            StatusId = s.StatusId,
            Status = s.Status?.Nome ?? string.Empty,
            DataSolicitacao = s.DataSolicitacao,
            NomeAnimal = s.NomeAnimal,
            DescricaoAnimal = s.DescricaoAnimal,
            MotivoEntrega = s.MotivoEntrega,
            EspecieDescricao = s.EspecieDescricao,
            RacaDescricao = s.RacaDescricao,
            IdadeEstimada = s.IdadeEstimada,
            ObservacaoAdmin = s.ObservacaoAdmin,
            AnimalId = s.AnimalId,
            Fotos = s.SolicitacaoFotos != null
                ? s.SolicitacaoFotos.Select(f => f.Url).ToList()
                : new List<string>()
        };

        public async Task<SolicitacaoDto> CriarSolicitacaoAsync(CreateSolicitacaoDto dto, int usuarioId)
        {
            var solicitacao = await CriarSolicitacaoInternaAsync(dto, usuarioId, dto.Fotos);
            return MapToDto(solicitacao);
        }

        public async Task<SolicitacaoDto> CriarSolicitacaoComFotosAsync(CreateSolicitacaoDto dto, int usuarioId)
        {
            var solicitacao = await CriarSolicitacaoInternaAsync(dto, usuarioId, dto.Fotos);
            return MapToDto(solicitacao);
        }

        public async Task<SolicitacaoDto?> GetByIdAsync(int id)
        {
            var s = await _solicitacaoRepository.Query()
                .Include(s => s.SolicitacaoFotos)
                .FirstOrDefaultAsync(s => s.Id == id);
            return s == null ? null : MapToDto(s);
        }

        public async Task<IEnumerable<SolicitacaoDto>> GetSolicitacoesUsuarioAsync(int usuarioId)
        {
            var list = await _solicitacaoRepository.GetSolicitacoesPorUsuarioAsync(usuarioId);
            return list.Select(MapToDto);
        }

        public async Task<IEnumerable<SolicitacaoDto>> GetSolicitacoesPendentesAsync()
        {
            var list = await _solicitacaoRepository.GetSolicitacoesPendentesAsync();
            return list.Select(MapToDto);
        }

        public async Task<SolicitacaoDto> AprovarSolicitacaoAsync(int id, int adminId, AprovarSolicitacaoDto dto)
        {
            var solicitacao = await _solicitacaoRepository.GetByIdAsync(id);
            if (solicitacao == null) throw new InvalidOperationException("Solicitação não encontrada.");

            var statusAprovado = await _statusRepository.ObterPorNomeAsync("Aprovado");
            if (statusAprovado == null) throw new InvalidOperationException("Status 'Aprovado' não encontrado.");

            var animalDto = dto.DadosAnimalCadastrado;
            var animalCriado = await _animalService.CreateAsync(animalDto, solicitacao.UsuarioId);

            solicitacao.StatusId = statusAprovado.Id;
            solicitacao.ObservacaoAdmin = dto.ObservacaoAdmin;
            solicitacao.AnimalId = animalCriado.Id;

            await _solicitacaoRepository.UpdateAsync(solicitacao);
            await _unitOfWork.SaveChangesAsync();

            return MapToDto(solicitacao);
        }

        public async Task<SolicitacaoDto> RejeitarSolicitacaoAsync(int id, int adminId, string motivo)
        {
            var solicitacao = await _solicitacaoRepository.GetByIdAsync(id);
            if (solicitacao == null) throw new InvalidOperationException("Solicitação não encontrada.");

            var statusRejeitado = await _statusRepository.ObterPorNomeAsync("Rejeitado");
            if (statusRejeitado == null) throw new InvalidOperationException("Status 'Rejeitado' não encontrado.");

            solicitacao.StatusId = statusRejeitado.Id;
            solicitacao.ObservacaoAdmin = motivo;

            await _solicitacaoRepository.UpdateAsync(solicitacao);
            await _unitOfWork.SaveChangesAsync();

            return MapToDto(solicitacao);
        }

        public async Task<SolicitacaoDto> AprovarSolicitacaoSimplesAsync(int id, string? observacaoAdmin)
        {
            // 1. Buscar solicitação incluindo as fotos
            var solicitacao = await _solicitacaoRepository.Query()
                .Include(s => s.SolicitacaoFotos)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (solicitacao == null) throw new InvalidOperationException("Solicitação não encontrada.");

            var statusAprovado = await _statusRepository.ObterPorNomeAsync("Aprovado");

            // 2. Criar o animal
            var createAnimalDto = new CreateAnimalDto
            {
                Nome = solicitacao.NomeAnimal,
                Descricao = solicitacao.DescricaoAnimal,
                MotivoDoacao = solicitacao.MotivoEntrega ?? "Doação voluntária",
                Sexo = "Macho", // Ideal seria ter isso no DTO de criação da solicitação
                Idade = ExtrairIdade(solicitacao.IdadeEstimada),
                PorteId = await MapearPorteId(solicitacao.PorteDescricao),
                EspecieId = await MapearEspecieId(solicitacao.EspecieDescricao),
                // Não passamos fotos aqui no DTO porque vamos migrar as da solicitação abaixo
            };

            var animalCriado = await _animalService.CreateAsync(createAnimalDto, solicitacao.UsuarioId, isAdmin: true);

            // 3. Migrar Fotos da Solicitação para a Galeria do Animal
            if (solicitacao.SolicitacaoFotos != null && solicitacao.SolicitacaoFotos.Any())
            {
                bool primeira = true;
                foreach (var sf in solicitacao.SolicitacaoFotos)
                {
                    var novaFoto = new Foto
                    {
                        Url = sf.Url,
                        IsPrincipal = primeira,
                        AnimalId = animalCriado.Id,
                        DataUpload = DateTime.UtcNow
                    };
                    await _fotoRepository.AddAsync(novaFoto);
                    primeira = false;
                }
            }

            // 4. Atualizar status da solicitação
            solicitacao.StatusId = statusAprovado.Id;
            solicitacao.ObservacaoAdmin = observacaoAdmin;
            solicitacao.AnimalId = animalCriado.Id;

            await _solicitacaoRepository.UpdateAsync(solicitacao);

            // 5. Um único SaveChanges para garantir a transação
            await _unitOfWork.SaveChangesAsync();

            // Notificação...
            return MapToDto(solicitacao);
        }

        private async Task<SolicitacaoEntrega> CriarSolicitacaoInternaAsync(CreateSolicitacaoDto dto, int usuarioId, List<IFormFile>? fotos)
        {
            var statusPendente = await _statusRepository.ObterPorNomeAsync("Pendente");
            if (statusPendente == null) throw new InvalidOperationException("Status 'Pendente' não encontrado.");

            var solicitacao = new SolicitacaoEntrega
            {
                UsuarioId = usuarioId,
                StatusId = statusPendente.Id,
                DataSolicitacao = DateTime.UtcNow,
                NomeAnimal = dto.NomeAnimal,
                DescricaoAnimal = dto.DescricaoAnimal,
                MotivoEntrega = dto.MotivoEntrega ?? string.Empty,
                EspecieDescricao = dto.EspecieDescricao,
                RacaDescricao = dto.RacaDescricao,
                IdadeEstimada = dto.IdadeEstimada,
                PorteDescricao = dto.PorteDescricao,
                CaracteristicasDescricao = dto.CaracteristicasDescricao
            };

            await _solicitacaoRepository.AddAsync(solicitacao);
            await _unitOfWork.SaveChangesAsync();

            if (fotos != null && fotos.Any())
            {
                foreach (var foto in fotos)
                {
                    var fotoDto = await _fotoService.UploadFotoSolicitacaoAsync(foto);
                    var solicitacaoFoto = new SolicitacaoFoto
                    {
                        Url = fotoDto.Url,
                        SolicitacaoEntregaId = solicitacao.Id,
                        DataUpload = DateTime.UtcNow
                    };
                    await _solicitacaoFotoRepository.AddAsync(solicitacaoFoto);
                }
                await _unitOfWork.SaveChangesAsync();
            }

            return solicitacao;
        }

        private int ExtrairIdade(string? idadeTexto)
        {
            if (string.IsNullOrWhiteSpace(idadeTexto)) return 0;
            var match = Regex.Match(idadeTexto, @"\d+");
            return match.Success && int.TryParse(match.Value, out int idade) ? idade : 0;
        }

        private async Task<int> MapearEspecieId(string? especieDescricao)
        {
            if (string.IsNullOrEmpty(especieDescricao)) return 1;
            var especies = await _especieService.GetAllAsync();
            var especie = especies.FirstOrDefault(e =>
                e.Nome.Equals(especieDescricao, StringComparison.OrdinalIgnoreCase));
            return especie?.Id ?? 1;
        }

        private async Task<int> MapearPorteId(string? porteDescricao)
        {
            if (string.IsNullOrEmpty(porteDescricao)) return 1;
            var portes = await _porteService.GetAllAsync();
            var porte = portes.FirstOrDefault(p =>
                p.Nome.Equals(porteDescricao, StringComparison.OrdinalIgnoreCase));
            return porte?.Id ?? 1;
        }
    }
}