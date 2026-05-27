using LigaDosPets.Application.Abstractions.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LigaDosPets.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FotosController : ControllerBase
    {
        private readonly IFotoService _fotoService;

        public FotosController(IFotoService fotoService)
        {
            _fotoService = fotoService;
        }

        [HttpGet("animal/{animalId}")]
        public async Task<IActionResult> GetFotosAnimal(int animalId)
        {
            var fotos = await _fotoService.GetFotosAnimalAsync(animalId);
            return Ok(fotos);
        }

        [HttpPost("animal/{animalId}")]
        [Authorize(Roles = "Administrador,Gerente")]
        public async Task<IActionResult> UploadFoto(int animalId, [FromForm] IFormFile arquivo, [FromForm] bool isPrincipal = false)
        {
            if (arquivo == null || arquivo.Length == 0)
                return BadRequest("Arquivo inválido.");

            var foto = await _fotoService.UploadFotoAsync(animalId, arquivo, isPrincipal);
            return Ok(foto);
        }

        [HttpPost("{fotoId}/definir-principal")]
        [Authorize(Roles = "Administrador,Gerente")]
        public async Task<IActionResult> DefinirFotoPrincipal(int fotoId)
        {
            await _fotoService.DefinirFotoPrincipalAsync(fotoId);
            return Ok();
        }

        [HttpDelete("{fotoId}")]
        [Authorize(Roles = "Administrador,Gerente")]
        public async Task<IActionResult> DeleteFoto(int fotoId)
        {
            await _fotoService.DeleteFotoAsync(fotoId);
            return NoContent();
        }
    }
}