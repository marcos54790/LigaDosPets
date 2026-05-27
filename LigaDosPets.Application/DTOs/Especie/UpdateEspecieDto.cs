using System.ComponentModel.DataAnnotations;

namespace LigaDosPets.Application.DTOs.Especie
{
    public class UpdateEspecieDto
    {
        [Required]
        [StringLength(50)]
        public string Nome { get; set; } = string.Empty;
    }
}