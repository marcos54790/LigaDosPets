using System.ComponentModel.DataAnnotations;

namespace LigaDosPets.Application.DTOs.Animal
{
    public class UpdateAnimalDto
    {
        [Required(ErrorMessage = "O nome do animal é obrigatório.")]
        [StringLength(100, MinimumLength = 2)]
        public string Nome { get; set; } = string.Empty;

        [Range(0, 30)]
        public int Idade { get; set; }

        [Required]
        public string Sexo { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Descricao { get; set; }

        [Required]
        public int PorteId { get; set; }

        [Required]
        public int EspecieId { get; set; }

        public int? RacaId { get; set; }

        public List<int> CaracteristicasIds { get; set; } = new();
    }
}