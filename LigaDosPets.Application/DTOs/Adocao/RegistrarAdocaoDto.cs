using System.ComponentModel.DataAnnotations;

namespace LigaDosPets.Application.DTOs.Adocao
{
    public class RegistrarAdocaoDto
    {
        [Required(ErrorMessage = "O animal é obrigatório.")]
        public int AnimalId { get; set; }

        [Required(ErrorMessage = "O adotante é obrigatório.")]
        public int UsuarioAdotanteId { get; set; }

        [Required(ErrorMessage = "É necessário confirmar a assinatura dos termos.")]
        public bool TermosAssinados { get; set; }

        [StringLength(500)]
        public string? Observacao { get; set; }
    }
}