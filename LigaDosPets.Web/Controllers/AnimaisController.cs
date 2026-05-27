using LigaDosPets.Application.Abstractions.Services;
using LigaDosPets.Application.DTOs.Animal;
using LigaDosPets.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LigaDosPets.Web.Controllers
{
    public class AnimaisController : Controller
    {
        private readonly IAnimalService _animalService;

        public AnimaisController(IAnimalService animalService)
        {
            _animalService = animalService;
        }

        // GET: /Animais (Catálogo público)
        public async Task<IActionResult> Index(AnimalFiltroDto? filtro)
        {
            var animaisDto = await _animalService.GetAnimaisDisponiveisAsync(filtro ?? new AnimalFiltroDto());

            var model = animaisDto.Select(a => new AnimalModel
            {
                Id = a.Id,
                Nome = a.Nome,
                Idade = a.Idade,
                EspecieNome = a.Especie,
                RacaNome = a.Raca ?? "SRD",
                FotoUrl = a.FotoPrincipalUrl ?? "/images/pets/placeholder.jpg",
                Sexo = a.Sexo,
                DisponivelParaAdocao = a.DisponivelParaAdocao
            }).ToList();

            return View(model);
        }
    }
}