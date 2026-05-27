namespace LigaDosPets.Application.DTOs.Usuario
{
    public class UsuarioListDto
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string TipoUsuario { get; set; } = string.Empty;
        public bool Ativo { get; set; }
    }
}