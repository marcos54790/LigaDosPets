using System.ComponentModel.DataAnnotations;

namespace LigaDosPets.Application.DTOs.Caracteristica
{
    public class UpdateCaracteristicaDto
    {
        [Required]
        [StringLength(50, MinimumLength = 2)]
        public string Nome { get; set; } = string.Empty;
    }
}