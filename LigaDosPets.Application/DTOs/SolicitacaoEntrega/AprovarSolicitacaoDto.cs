using LigaDosPets.Application.DTOs.Animal;
using System.ComponentModel.DataAnnotations;

namespace LigaDosPets.Application.DTOs.SolicitacaoEntrega
{
    public class AprovarSolicitacaoDto
    {
        [StringLength(500)]
        public string? ObservacaoAdmin { get; set; }

        [Required(ErrorMessage = "Os dados do animal a ser cadastrado são obrigatórios.")]
        public CreateAnimalDto DadosAnimalCadastrado { get; set; } = null!;
    }
}