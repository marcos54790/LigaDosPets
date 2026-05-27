namespace LigaDosPets.Domain.Entities
{
    public class AnimalCaracteristica
    {
        public int AnimalId { get; set; }
        public int CaracteristicaId { get; set; }

        public Animal Animal { get; set; } = null!;
        public Caracteristica Caracteristica { get; set; } = null!;
    }
}