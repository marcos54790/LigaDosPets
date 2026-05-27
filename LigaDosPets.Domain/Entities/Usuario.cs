namespace LigaDosPets.Domain.Entities
{
    public class Usuario
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Telefone { get; set; }

        public bool PerfilDoadorCompleto { get; set; } = false;
        public string? Cep { get; set; }
        public string? Logradouro { get; set; }
        public string? Numero { get; set; }
        public string? Bairro { get; set; }
        public string? Cidade { get; set; }
        public string? Estado { get; set; }

        public int TipoUsuarioId { get; set; }
        public DateTime DataCadastro { get; set; } = DateTime.UtcNow;
        public bool Ativo { get; set; } = true;
        public string ApplicationUserId { get; set; } = string.Empty;

        public TipoUsuario TipoUsuario { get; set; } = null!;
        public ICollection<Animal> AnimaisCadastrados { get; set; } = new List<Animal>();
        public ICollection<Agendamento> Agendamentos { get; set; } = new List<Agendamento>();
        public ICollection<SolicitacaoEntrega> Solicitacoes { get; set; } = new List<SolicitacaoEntrega>();
        public ICollection<Adocao> Adocoes { get; set; } = new List<Adocao>();
        public ICollection<Adocao> AdocoesConfirmadas { get; set; } = new List<Adocao>();
        public ICollection<Conversa> ConversasComoA { get; set; } = new List<Conversa>();
        public ICollection<Conversa> ConversasComoB { get; set; } = new List<Conversa>();
        public ICollection<Mensagem> MensagensEnviadas { get; set; } = new List<Mensagem>();
        public ICollection<Notificacao> Notificacoes { get; set; } = new List<Notificacao>();
    }
}