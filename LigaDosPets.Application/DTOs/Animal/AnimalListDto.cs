// Application/DTOs/Animal/AnimalListDto.cs
namespace LigaDosPets.Application.DTOs.Animal
{
    public class AnimalListDto
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public int? Idade { get; set; }
        public string Sexo { get; set; } = string.Empty;
        public string Porte { get; set; } = string.Empty;
        public string Especie { get; set; } = string.Empty;
        public string? Raca { get; set; }
        public string? FotoPrincipalUrl { get; set; }
        public bool DisponivelParaAdocao { get; set; }
        public bool AguardandoValidacao { get; set; }
        public DateTime DataCadastro { get; set; } // ✅ necessário para gráfico de meses e destaques
    }
}