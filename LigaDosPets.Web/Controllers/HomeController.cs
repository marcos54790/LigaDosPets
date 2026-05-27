using LigaDosPets.Application.Abstractions.Services;
using LigaDosPets.Application.DTOs.Animal;
using LigaDosPets.Web.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace LigaDosPets.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IAnimalService _animalService;
        private readonly IAdocaoService _adocaoService;
        private readonly IEspecieService _especieService;

        public HomeController(
            IAnimalService animalService,
            IAdocaoService adocaoService,
            IEspecieService especieService)
        {
            _animalService = animalService;
            _adocaoService = adocaoService;
            _especieService = especieService;
        }

        public async Task<IActionResult> Index()
        {
            var animaisDisponiveis = await _animalService.GetAnimaisDisponiveisAsync(new AnimalFiltroDto());
            var adocoes = await _adocaoService.GetHistoricoAsync();
            var especies = await _especieService.GetAllAsync();

            var destaques = animaisDisponiveis
                .OrderByDescending(a => a.DataCadastro)
                .Take(4)
                .ToList();

            var model = new HomeViewModel
            {
                TotalAnimaisDisponiveis = animaisDisponiveis.Count(),
                TotalAdotados = adocoes.Count(),
                TotalEspecies = especies.Count(),
                AnimaisDestaque = destaques
            };

            return View(model);
        }

        public IActionResult Privacy() => View();

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() => View();
        public IActionResult About()
        {
            return View();
        }
    }
}