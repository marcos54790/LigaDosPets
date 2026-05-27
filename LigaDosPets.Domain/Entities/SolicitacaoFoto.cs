namespace LigaDosPets.Domain.Entities
{
    public class SolicitacaoFoto
    {
        public int Id { get; set; }
        public string Url { get; set; } = string.Empty;
        public int SolicitacaoEntregaId { get; set; }
        public DateTime DataUpload { get; set; } = DateTime.UtcNow;
        public SolicitacaoEntrega SolicitacaoEntrega { get; set; } = null!;
    }
}