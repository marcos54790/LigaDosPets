// Web/Models/ViewModels/DashboardModel.cs
namespace LigaDosPets.Web.Models.ViewModels
{
    public class DashboardModel
    {
        // ── Cards de estatística ──────────────────────────────
        public int TotalAnimais { get; set; }
        public int TotalAdotados { get; set; }
        public int TotalPendentesValidacao { get; set; }
        public int TotalUsuarios { get; set; }
        public int TotalAgendamentos { get; set; }
        public int TotalSolicitacoesPendentes { get; set; }

        // ── Gráfico: Distribuição por Espécie (doughnut) ─────
        public List<string> LabelsEspecies { get; set; } = new();
        public List<int> ValoresEspecies { get; set; } = new();

        // ── Gráfico: Distribuição por Porte (bar) ────────────
        public List<string> LabelsPortes { get; set; } = new();
        public List<int> ValoresPortes { get; set; } = new();

        // ── Gráfico: Animais por Sexo (doughnut pequeno) ─────
        public int TotalMachos { get; set; }
        public int TotalFemeas { get; set; }

        // ── Gráfico: Cadastros por mês (line) ────────────────
        public List<string> LabelsMeses { get; set; } = new();
        public List<int> ValoresCadastrosMes { get; set; } = new();
    }
}