using System.ComponentModel.DataAnnotations;

namespace LigaDosPets.Application.DTOs.Usuario
{
    public class UpdateUsuarioDto
    {
        [Required(ErrorMessage = "O nome é obrigatório.")]
        [StringLength(100)]
        public string Nome { get; set; } = string.Empty;

        [Phone(ErrorMessage = "Telefone inválido.")]
        public string? Telefone { get; set; }
    }
}