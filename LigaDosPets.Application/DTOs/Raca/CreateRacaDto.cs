using System.ComponentModel.DataAnnotations;

namespace LigaDosPets.Application.DTOs.Raca
{
    public class CreateRacaDto
    {
        [Required]
        [StringLength(50)]
        public string Nome { get; set; } = string.Empty;

        [Required(ErrorMessage = "A espécie é obrigatória.")]
        public int EspecieId { get; set; }
    }
}