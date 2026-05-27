namespace LigaDosPets.Domain.Entities
{
    public class Foto
    {
        public int Id { get; set; }
        public string Url { get; set; } = string.Empty;
        public bool IsPrincipal { get; set; } = false;
        public int AnimalId { get; set; }
        public DateTime DataUpload { get; set; } = DateTime.UtcNow;

        public Animal Animal { get; set; } = null!;
    }
}