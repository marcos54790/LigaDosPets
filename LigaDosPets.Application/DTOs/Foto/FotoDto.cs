namespace LigaDosPets.Application.DTOs.Foto
{
    public class FotoDto
    {
        public int Id { get; set; }
        public string Url { get; set; } = string.Empty;
        public bool IsPrincipal { get; set; }
        public DateTime DataUpload { get; set; }
    }
}