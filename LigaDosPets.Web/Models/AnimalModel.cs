namespace LigaDosPets.Web.Models
{
    public class AnimalModel
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public int? Idade { get; set; }
        public string EspecieNome { get; set; } = string.Empty;
        public string RacaNome { get; set; } = string.Empty;
        public string FotoUrl { get; set; } = string.Empty;
        public string Sexo { get; set; } = string.Empty;
        public bool DisponivelParaAdocao { get; set; } = true;
        public bool AguardandoValidacao { get; set; }
        public int UsuarioCadastroId { get; set; }
    }
}