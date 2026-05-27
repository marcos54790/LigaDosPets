namespace LigaDosPets.Domain.Entities
{
    /// <summary>
    /// Registro formal de adoção de um animal.
    /// Criado pelo Admin na dashboard após confirmar que a adoção aconteceu.
    /// Ao criar, o Admin também deve setar Animal.DisponivelParaAdocao = false.
    /// </summary>
    public class Adocao
    {
        public int Id { get; set; }
        public int AnimalId { get; set; }
        public int UsuarioAdotanteId { get; set; }

        /// <summary>Admin que registrou a adoção no sistema.</summary>
        public int AdminConfirmouId { get; set; }

        public DateTime DataAdocao { get; set; } = DateTime.UtcNow;
        public string? Observacao { get; set; }

        /// <summary>
        /// Indica se o termo de adoção foi assinado.
        /// Útil para compliance e relatórios.
        /// </summary>
        public bool TermosAssinados { get; set; } = false;

        // Navegação
        public Animal Animal { get; set; } = null!;
        public Usuario UsuarioAdotante { get; set; } = null!;
        public Usuario AdminConfirmou { get; set; } = null!;
    }
}