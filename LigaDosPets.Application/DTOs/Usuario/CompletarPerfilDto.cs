using System.ComponentModel.DataAnnotations;

namespace LigaDosPets.Application.DTOs.Usuario
{
    public class CompletarPerfilDto
    {
        [Required(ErrorMessage = "O Telefone é necessário para contato da ONG.")]
        [StringLength(15)]
        public string Telefone { get; set; } = string.Empty;

        [Required(ErrorMessage = "O CEP é fundamental para localizar a origem do animal.")]
        [StringLength(9)]
        public string Cep { get; set; } = string.Empty;

        public string Logradouro { get; set; } = string.Empty;
        public string Bairro { get; set; } = string.Empty;
        public string Cidade { get; set; } = string.Empty;
        public string Estado { get; set; } = string.Empty;

        [Required(ErrorMessage = "O número da residência é obrigatório.")]
        [StringLength(100)]
        public string Numero { get; set; } = string.Empty;

        public bool AceitouTermos { get; set; }
    }
}