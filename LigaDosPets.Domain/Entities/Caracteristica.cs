namespace LigaDosPets.Domain.Entities
{
    public class Caracteristica
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;

        public ICollection<AnimalCaracteristica> AnimalCaracteristicas { get; set; } = new List<AnimalCaracteristica>();
    }
}
