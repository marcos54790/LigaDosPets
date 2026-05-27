namespace LigaDosPets.Domain.Entities
{
    public class StatusSolicitacao
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;

        public ICollection<SolicitacaoEntrega> Solicitacoes { get; set; } = new List<SolicitacaoEntrega>();
    }
}