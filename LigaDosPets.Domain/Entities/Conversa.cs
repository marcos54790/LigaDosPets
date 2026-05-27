namespace LigaDosPets.Domain.Entities
{
    /// <summary>
    /// Canal de comunicação entre dois usuários, geralmente sobre um animal específico.
    /// Evita duplicatas: antes de criar, verifique se já existe uma conversa
    /// entre os mesmos dois usuários sobre o mesmo animal.
    /// </summary>
    public class Conversa
    {
        public int Id { get; set; }

        /// <summary>Um dos participantes (quem iniciou).</summary>
        public int UsuarioAId { get; set; }

        /// <summary>O outro participante (ex: Admin do abrigo).</summary>
        public int UsuarioBId { get; set; }

        /// <summary>Animal sobre o qual a conversa foi aberta. Opcional.</summary>
        public int? AnimalId { get; set; }

        public DateTime DataCriacao { get; set; } = DateTime.UtcNow;

        /// <summary>Atualizado a cada nova mensagem — facilita ordenar conversas recentes.</summary>
        public DateTime? UltimaMensagemEm { get; set; }

        // Navegação
        public Usuario UsuarioA { get; set; } = null!;
        public Usuario UsuarioB { get; set; } = null!;
        public Animal? Animal { get; set; }
        public ICollection<Mensagem> Mensagens { get; set; } = new List<Mensagem>();
    }
}