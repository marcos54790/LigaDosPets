using LigaDosPets.Application.DTOs.Foto;
using Microsoft.AspNetCore.Http;

namespace LigaDosPets.Application.Abstractions.Services
{
    public interface IFotoService
    {
        Task<IEnumerable<FotoDto>> GetFotosAnimalAsync(int animalId);
        Task<FotoDto> UploadFotoAsync(int animalId, IFormFile arquivo, bool isPrincipal);
        Task DefinirFotoPrincipalAsync(int fotoId);
        Task DeleteFotoAsync(int fotoId);
        Task<FotoDto> UploadFotoSolicitacaoAsync(IFormFile arquivo);
    }
}