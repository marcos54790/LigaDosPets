namespace LigaDosPets.Application.DTOs.Notificacao
{
    public class NotificacaoDto
    {
        public int Id { get; set; }
        public string Tipo { get; set; } = string.Empty;
        public string Titulo { get; set; } = string.Empty;
        public string Mensagem { get; set; } = string.Empty;
        public bool Lida { get; set; }
        public string? UrlAcao { get; set; }
        public DateTime DataCriacao { get; set; }
    }
}