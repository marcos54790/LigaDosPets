namespace LigaDosPets.Domain.Entities
{
    public class Porte
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public ICollection<Animal> Animais { get; set; } = new List<Animal>();
    }
}