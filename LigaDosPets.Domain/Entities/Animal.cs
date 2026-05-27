namespace LigaDosPets.Domain.Entities
{
    /// <summary>
    /// Animal cadastrado no abrigo, seja pelo próprio abrigo (Admin)
    /// ou por um usuário via SolicitacaoEntrega aprovada.
    /// </summary>
    public class Animal
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;

        /// <summary>Idade em anos. Pode ser nulo se estimado apenas em meses.</summary>
        public int? Idade { get; set; }

        /// <summary>M = Macho, F = Fêmea</summary>
        public string Sexo { get; set; } = string.Empty;

        public string? Descricao { get; set; }

        /// <summary>
        /// Motivo pelo qual o animal entrou no abrigo.
        /// Ex: Abandono, Doação voluntária, Resgate.
        /// </summary>
        public string? MotivoEntrada { get; set; }

        /// <summary>
        /// Quando false, o animal foi adotado ou removido do catálogo.
        /// Atualizado pelo Admin na dashboard após registrar Adocao.
        /// </summary>
        public bool DisponivelParaAdocao { get; set; } = true;

        /// <summary>
        /// Quando true, o animal ainda não foi validado pelo Admin.
        /// Animais nesse estado não aparecem no catálogo público.
        /// </summary>
        public bool AguardandoValidacao { get; set; } = true;

        public DateTime DataCadastro { get; set; } = DateTime.UtcNow;

        // Chaves estrangeiras
        public int PorteId { get; set; }
        public int EspecieId { get; set; }
        public int? RacaId { get; set; }
        public int UsuarioCadastroId { get; set; }

        // Navegação
        public Porte Porte { get; set; } = null!;
        public Especie Especie { get; set; } = null!;
        public Raca? Raca { get; set; }
        public Usuario UsuarioCadastro { get; set; } = null!;

        public ICollection<Foto> Fotos { get; set; } = new List<Foto>();
        public ICollection<AnimalCaracteristica> AnimalCaracteristicas { get; set; } = new List<AnimalCaracteristica>();
        public ICollection<Agendamento> Agendamentos { get; set; } = new List<Agendamento>();
        public ICollection<SolicitacaoEntrega> SolicitacoesEntrega { get; set; } = new List<SolicitacaoEntrega>();
        public ICollection<Adocao> Adocoes { get; set; } = new List<Adocao>();
        public ICollection<Conversa> Conversas { get; set; } = new List<Conversa>();
    }
}