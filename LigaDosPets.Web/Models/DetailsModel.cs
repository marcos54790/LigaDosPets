// Web/Models/DetailsModel.cs
namespace LigaDosPets.Web.Models
{
    public class DetailsModel
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public int? Idade { get; set; }
        public string Sexo { get; set; } = string.Empty;
        public string? Descricao { get; set; }
        public string EspecieNome { get; set; } = string.Empty;
        public string RacaNome { get; set; } = string.Empty;
        public string PorteNome { get; set; } = string.Empty;
        public string UsuarioCadastroNome { get; set; } = string.Empty;
        public bool DisponivelParaAdocao { get; set; }
        public List<FotoModel> Fotos { get; set; } = new();
        public List<string> HorariosDisponiveis { get; set; } = new();

        /// <summary>Características do animal (Castrado, Vacinado, etc.)</summary>
        public List<string> Caracteristicas { get; set; } = new();

        /// <summary>
        /// Horários já ocupados na data de hoje.
        /// Preenchido dinamicamente via JS ao trocar a data no modal.
        /// </summary>
        public List<string> HorariosOcupadosHoje { get; set; } = new();
    }

    public class FotoModel
    {
        public string Url { get; set; } = string.Empty;
    }
}