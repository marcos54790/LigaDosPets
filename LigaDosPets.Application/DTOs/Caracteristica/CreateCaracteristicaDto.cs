using System.ComponentModel.DataAnnotations;

namespace LigaDosPets.Application.DTOs.Caracteristica
{
    public class CreateCaracteristicaDto
    {
        [Required(ErrorMessage = "O nome da característica é obrigatório.")]
        [StringLength(50, MinimumLength = 2)]
        public string Nome { get; set; } = string.Empty;
    }
}