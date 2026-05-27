using System.ComponentModel.DataAnnotations;

namespace LigaDosPets.Application.DTOs.Especie
{
    public class CreateEspecieDto
    {
        [Required(ErrorMessage = "O nome da espécie é obrigatório.")]
        [StringLength(50)]
        public string Nome { get; set; } = string.Empty;
    }
}