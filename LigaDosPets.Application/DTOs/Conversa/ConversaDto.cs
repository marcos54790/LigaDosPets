namespace LigaDosPets.Application.DTOs.Conversa
{
    public class ConversaDto
    {
        public int Id { get; set; }
        public int UsuarioAId { get; set; }
        public int UsuarioBId { get; set; }
        public string NomeOutroUsuario { get; set; } = string.Empty;
        public int? AnimalId { get; set; }
        public string? NomeAnimal { get; set; }
        public DateTime? UltimaMensagemEm { get; set; }
        public int MensagensNaoLidas { get; set; }
    }
}