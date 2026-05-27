namespace LigaDosPets.Domain.Entities
{
    public class Agendamento
    {
        public int Id { get; set; }
        public int AnimalId { get; set; }
        public int UsuarioId { get; set; }
        public int StatusAgendamentoId { get; set; }
        public DateTime DataVisita { get; set; }
        public string? Observacao { get; set; }
        public DateTime DataCriacao { get; set; } = DateTime.UtcNow;

        public Animal Animal { get; set; } = null!;
        public Usuario Usuario { get; set; } = null!;
        public StatusAgendamento StatusAgendamento { get; set; } = null!;
    }
}