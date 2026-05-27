namespace LigaDosPets.Domain.Entities
{
    public class StatusAgendamento
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;

        public ICollection<Agendamento> Agendamentos { get; set; } = new List<Agendamento>();
    }
}