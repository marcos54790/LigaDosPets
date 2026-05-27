using System.ComponentModel.DataAnnotations;

namespace LigaDosPets.Web.Models
{
    public class UsuarioViewModel
    {
        [Required(ErrorMessage = "Nome é obrigatório")]
        public string Nome { get; set; } = string.Empty;

        [Required(ErrorMessage = "E-mail é obrigatório")]
        [EmailAddress(ErrorMessage = "E-mail inválido")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Senha é obrigatória")]
        [DataType(DataType.Password)]
        [MinLength(6, ErrorMessage = "Mínimo 6 caracteres")]
        public string Senha { get; set; } = string.Empty;

        [Phone]
        public string? Telefone { get; set; }

        [Required(ErrorMessage = "Tipo de usuário é obrigatório")]
        public int TipoUsuarioId { get; set; }
    }
}