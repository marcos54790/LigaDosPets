using LigaDosPets.Application.Abstractions;
using LigaDosPets.Application.Abstractions.Repositories;
using LigaDosPets.Application.Abstractions.Services;
using LigaDosPets.Application.DTOs.Foto;
using LigaDosPets.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;

namespace LigaDosPets.Application.Services
{
    public class FotoService : IFotoService
    {
        private readonly IFotoRepository _fotoRepository;
        private readonly IAnimalRepository _animalRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHostEnvironment _environment;

        public FotoService(
            IFotoRepository fotoRepository,
            IAnimalRepository animalRepository,
            IUnitOfWork unitOfWork,
            IHostEnvironment environment)
        {
            _fotoRepository = fotoRepository;
            _animalRepository = animalRepository;
            _unitOfWork = unitOfWork;
            _environment = environment;
        }

        private static FotoDto MapToDto(Foto f) => new()
        {
            Id = f.Id,
            Url = f.Url,
            IsPrincipal = f.IsPrincipal,
            DataUpload = f.DataUpload
        };

        public async Task<IEnumerable<FotoDto>> GetFotosAnimalAsync(int animalId)
        {
            var fotos = await _fotoRepository.GetFotosPorAnimalAsync(animalId);
            return fotos.Select(MapToDto);
        }

        public async Task<FotoDto> UploadFotoAsync(int animalId, IFormFile arquivo, bool isPrincipal)
        {
            var animal = await _animalRepository.GetByIdAsync(animalId);
            if (animal == null) throw new InvalidOperationException("Animal não encontrado.");

            // Define o caminho da pasta wwwroot/imagens
            var uploadsFolder = Path.Combine(_environment.ContentRootPath, "wwwroot", "imagens");
            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            // Gera nome único para o arquivo
            var extensao = Path.GetExtension(arquivo.FileName);
            var nomeArquivo = $"{Guid.NewGuid()}{extensao}";
            var caminhoCompleto = Path.Combine(uploadsFolder, nomeArquivo);

            // Salva o arquivo no disco
            using (var stream = new FileStream(caminhoCompleto, FileMode.Create))
            {
                await arquivo.CopyToAsync(stream);
            }

            // URL relativa (acessível pelo navegador)
            var urlRelativa = $"/imagens/{nomeArquivo}";

            var foto = new Foto
            {
                Url = urlRelativa,
                IsPrincipal = isPrincipal,
                AnimalId = animalId,
                DataUpload = DateTime.UtcNow
            };

            if (isPrincipal)
            {
                var principalAtual = await _fotoRepository.GetFotoPrincipalAsync(animalId);
                if (principalAtual != null)
                {
                    principalAtual.IsPrincipal = false;
                    await _fotoRepository.UpdateAsync(principalAtual);
                }
            }

            await _fotoRepository.AddAsync(foto);
            await _unitOfWork.SaveChangesAsync();

            return MapToDto(foto);
        }

        public async Task DefinirFotoPrincipalAsync(int fotoId)
        {
            await _fotoRepository.DefinirFotoPrincipalAsync(fotoId);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteFotoAsync(int fotoId)
        {
            var foto = await _fotoRepository.GetByIdAsync(fotoId);
            if (foto == null) throw new InvalidOperationException("Foto não encontrada.");

            // Remove o arquivo físico, se existir
            if (!string.IsNullOrEmpty(foto.Url))
            {
                var caminhoFisico = Path.Combine(_environment.ContentRootPath, "wwwroot", foto.Url.TrimStart('/'));
                if (File.Exists(caminhoFisico))
                    File.Delete(caminhoFisico);
            }

            await _fotoRepository.DeleteAsync(foto);
            await _unitOfWork.SaveChangesAsync();
        }
        public async Task<FotoDto> UploadFotoSolicitacaoAsync(IFormFile arquivo)
        {
            var uploadsFolder = Path.Combine(_environment.ContentRootPath, "wwwroot", "imagens");
            if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);

            var extensao = Path.GetExtension(arquivo.FileName);
            var nomeArquivo = $"{Guid.NewGuid()}{extensao}";
            var caminhoCompleto = Path.Combine(uploadsFolder, nomeArquivo);

            using (var stream = new FileStream(caminhoCompleto, FileMode.Create))
            {
                await arquivo.CopyToAsync(stream);
            }

            var urlRelativa = $"/imagens/{nomeArquivo}";
            return new FotoDto { Url = urlRelativa, IsPrincipal = false, DataUpload = DateTime.UtcNow };
        }
    }
}