using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace LigaDosPets.Application.DTOs.Animal
{
    public class CreateAnimalDto
    {
        [Required(ErrorMessage = "O nome do animal é obrigatório.")]
        [StringLength(100, MinimumLength = 2)]
        public string Nome { get; set; } = string.Empty;

        [Range(0, 30)]
        public int Idade { get; set; }

        [Required(ErrorMessage = "O sexo é obrigatório.")]
        public string Sexo { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Descricao { get; set; }

        [StringLength(200)]
        public string? MotivoDoacao { get; set; }

        [Required(ErrorMessage = "O porte é obrigatório.")]
        public int PorteId { get; set; }

        [Required(ErrorMessage = "A espécie é obrigatória.")]
        public int EspecieId { get; set; }

        public int? RacaId { get; set; }

        public List<int> CaracteristicasIds { get; set; } = new();

        public List<IFormFile>? Fotos { get; set; }
    }
}