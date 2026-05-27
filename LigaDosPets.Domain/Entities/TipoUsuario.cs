namespace LigaDosPets.Domain.Entities
{
    public class TipoUsuario
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;

        public ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();
    }
}