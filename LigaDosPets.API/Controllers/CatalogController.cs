using LigaDosPets.Application.Abstractions.Services;
using Microsoft.AspNetCore.Mvc;

namespace LigaDosPets.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CatalogController : ControllerBase
    {
        private readonly IPorteService _porteService;
        private readonly IEspecieService _especieService;
        private readonly IRacaService _racaService;
        private readonly ICaracteristicaService _caracteristicaService;

        public CatalogController(
            IPorteService porteService,
            IEspecieService especieService,
            IRacaService racaService,
            ICaracteristicaService caracteristicaService)
        {
            _porteService = porteService;
            _especieService = especieService;
            _racaService = racaService;
            _caracteristicaService = caracteristicaService;
        }

        [HttpGet("portes")]
        public async Task<IActionResult> GetPortes()
        {
            var portes = await _porteService.GetAllAsync();
            return Ok(portes);
        }

        [HttpGet("especies")]
        public async Task<IActionResult> GetEspecies()
        {
            var especies = await _especieService.GetAllAsync();
            return Ok(especies);
        }

        [HttpGet("racas")]
        public async Task<IActionResult> GetRacas([FromQuery] int? especieId)
        {
            if (especieId.HasValue)
            {
                var racas = await _racaService.GetByEspecieAsync(especieId.Value);
                return Ok(racas);
            }
            var todasRacas = await _racaService.GetAllAsync();
            return Ok(todasRacas);
        }

        [HttpGet("caracteristicas")]
        public async Task<IActionResult> GetCaracteristicas()
        {
            var caracteristicas = await _caracteristicaService.GetAllAsync();
            return Ok(caracteristicas);
        }
    }
}