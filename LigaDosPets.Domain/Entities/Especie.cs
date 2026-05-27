namespace LigaDosPets.Domain.Entities
{
    public class Especie
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;

        public ICollection<Raca> Racas { get; set; } = new List<Raca>();
        public ICollection<Animal> Animais { get; set; } = new List<Animal>();
    }
}