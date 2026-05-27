using System.ComponentModel.DataAnnotations;

namespace LigaDosPets.Application.DTOs.Porte
{
    public class UpdatePorteDto
    {
        [Required]
        [StringLength(50)]
        public string Nome { get; set; } = string.Empty;
    }
}