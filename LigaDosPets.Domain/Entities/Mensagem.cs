namespace LigaDosPets.Domain.Entities
{
    /// <summary>
    /// Mensagem enviada dentro de uma Conversa.
    /// O chat é thread-based: sempre via Conversa, nunca direto entre usuários.
    /// </summary>
    public class Mensagem
    {
        public int Id { get; set; }
        public int ConversaId { get; set; }
        public int RemetenteId { get; set; }
        public string Conteudo { get; set; } = string.Empty;
        public bool Lida { get; set; } = false;
        public DateTime DataEnvio { get; set; } = DateTime.UtcNow;

        public Conversa Conversa { get; set; } = null!;
        public Usuario Remetente { get; set; } = null!;
    }
}