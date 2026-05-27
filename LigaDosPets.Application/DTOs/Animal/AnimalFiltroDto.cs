namespace LigaDosPets.Application.DTOs.Animal
{
    public class AnimalFiltroDto
    {
        public int? EspecieId { get; set; }
        public int? PorteId { get; set; }
        public int? RacaId { get; set; }
        public string? Sexo { get; set; }
        public int? IdadeMinima { get; set; }
        public int? IdadeMaxima { get; set; }
    }
}