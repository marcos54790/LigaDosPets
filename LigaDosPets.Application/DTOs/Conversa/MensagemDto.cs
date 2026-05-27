namespace LigaDosPets.Application.DTOs.Conversa
{
    public class MensagemDto
    {
        public int Id { get; set; }
        public int ConversaId { get; set; }
        public int RemetenteId { get; set; }
        public string NomeRemetente { get; set; } = string.Empty;
        public string Conteudo { get; set; } = string.Empty;
        public bool Lida { get; set; }
        public DateTime DataEnvio { get; set; }
    }
}