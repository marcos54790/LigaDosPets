namespace LigaDosPets.Domain.Entities
{
    public class Raca
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public int EspecieId { get; set; }

        public Especie Especie { get; set; } = null!;
        public ICollection<Animal> Animais { get; set; } = new List<Animal>();
    }
}