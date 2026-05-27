namespace LigaDosPets.Application.DTOs.Conversa
{
    public class ConversaResumoDto
    {
        public int Id { get; set; }
        public string NomeOutroUsuario { get; set; } = string.Empty;
        public string? NomeAnimal { get; set; }
        public string? UltimaMensagem { get; set; }
        public DateTime? UltimaMensagemEm { get; set; }
        public int NaoLidas { get; set; }
    }
}