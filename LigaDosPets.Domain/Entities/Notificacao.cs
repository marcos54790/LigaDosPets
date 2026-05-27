namespace LigaDosPets.Domain.Entities
{
    /// <summary>
    /// Notificação gerada pelo sistema para um usuário.
    /// Disparada nos eventos: agendamento confirmado/cancelado,
    /// solicitação aprovada/rejeitada, nova mensagem, adoção registrada.
    /// </summary>
    public class Notificacao
    {
        public int Id { get; set; }
        public int UsuarioId { get; set; }

        /// <summary>
        /// Tipo da notificação para agrupamento/filtragem.
        /// Exemplos: "AgendamentoConfirmado", "SolicitacaoAprovada",
        ///           "NovaMensagem", "AdocaoRegistrada"
        /// </summary>
        public string Tipo { get; set; } = string.Empty;

        public string Titulo { get; set; } = string.Empty;
        public string Mensagem { get; set; } = string.Empty;
        public bool Lida { get; set; } = false;

        /// <summary>URL relativa para redirecionar ao clicar na notificação.</summary>
        public string? UrlAcao { get; set; }

        public DateTime DataCriacao { get; set; } = DateTime.UtcNow;

        public Usuario Usuario { get; set; } = null!;
    }
}