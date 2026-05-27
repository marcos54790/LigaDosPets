// Web/Models/ViewModels/HomeViewModel.cs
using LigaDosPets.Application.DTOs.Animal;

namespace LigaDosPets.Web.Models.ViewModels
{
    public class HomeViewModel
    {
        public int TotalAnimaisDisponiveis { get; set; }
        public int TotalAdotados { get; set; }
        public int TotalEspecies { get; set; }
        public List<AnimalListDto> AnimaisDestaque { get; set; } = new();
    }
}