using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace LigaDosPets.Application.DTOs.SolicitacaoEntrega
{
    public class CreateSolicitacaoDto
    {
        [Required(ErrorMessage = "O nome do animal é obrigatório.")]
        [StringLength(100)]
        public string NomeAnimal { get; set; } = string.Empty;

        [StringLength(500)]
        public string? DescricaoAnimal { get; set; }

        [StringLength(300)]
        public string? MotivoEntrega { get; set; }

        [StringLength(50)]
        public string? EspecieDescricao { get; set; }

        [StringLength(50)]
        public string? RacaDescricao { get; set; }

        [StringLength(50)]
        public string? IdadeEstimada { get; set; }
        public List<IFormFile>? Fotos { get; set; }
        public string? PorteDescricao { get; set; }
        public string? CaracteristicasDescricao { get; set; }
    }
}