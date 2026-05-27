namespace LigaDosPets.Application.DTOs.Raca
{
    public class RacaDto
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public int EspecieId { get; set; }
        public string NomeEspecie { get; set; } = string.Empty;
    }
}