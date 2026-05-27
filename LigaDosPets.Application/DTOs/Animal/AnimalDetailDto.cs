namespace LigaDosPets.Application.DTOs.Animal
{
    public class AnimalDetailDto : AnimalListDto
    {
        public string? Descricao { get; set; }
        public string? MotivoEntrada { get; set; }
        public List<string> Fotos { get; set; } = new();
        public List<string> Caracteristicas { get; set; } = new();
        public string NomeUsuarioCadastro { get; set; } = string.Empty;
        public int UsuarioCadastroId { get; set; }
    }
}