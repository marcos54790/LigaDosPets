namespace LigaDosPets.Application.DTOs.Adocao
{
    public class AdocaoDto
    {
        public int Id { get; set; }
        public int AnimalId { get; set; }
        public string NomeAnimal { get; set; } = string.Empty;
        public int UsuarioAdotanteId { get; set; }
        public string NomeAdotante { get; set; } = string.Empty;
        public int AdminConfirmouId { get; set; }
        public string NomeAdmin { get; set; } = string.Empty;
        public DateTime DataAdocao { get; set; }
        public bool TermosAssinados { get; set; }
        public string? Observacao { get; set; }
    }
}