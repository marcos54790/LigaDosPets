using System.ComponentModel.DataAnnotations;

namespace LigaDosPets.Web.Models
{
    public class ChangePasswordModel
    {
        [Required(ErrorMessage = "Senha atual é obrigatória")]
        [DataType(DataType.Password)]
        public string CurrentPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "Nova senha é obrigatória")]
        [DataType(DataType.Password)]
        [MinLength(6, ErrorMessage = "A senha deve ter no mínimo 6 caracteres")]
        public string NewPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "Confirmação de senha é obrigatória")]
        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "As senhas não conferem")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}