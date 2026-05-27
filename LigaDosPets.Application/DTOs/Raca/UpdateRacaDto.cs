using System.ComponentModel.DataAnnotations;

namespace LigaDosPets.Application.DTOs.Raca
{
    public class UpdateRacaDto
    {
        [Required]
        [StringLength(50)]
        public string Nome { get; set; } = string.Empty;
    }
}