namespace LigaDosPets.Application.DTOs.Agendamento
{
    public class AgendamentoDto
    {
        public int Id { get; set; }
        public int AnimalId { get; set; }
        public string NomeAnimal { get; set; } = string.Empty;
        public int UsuarioId { get; set; }
        public string NomeUsuario { get; set; } = string.Empty;
        public int StatusAgendamentoId { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime DataVisita { get; set; }
        public string? Observacao { get; set; }
        public DateTime DataCriacao { get; set; }
    }
}