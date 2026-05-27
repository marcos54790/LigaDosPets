namespace LigaDosPets.Application.DTOs.Usuario
{
    public class UsuarioPerfilDto
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Telefone { get; set; }
        public string? Cep { get; set; }
        public string TipoUsuario { get; set; } = string.Empty;
        public DateTime DataCadastro { get; set; }
        public bool Ativo { get; set; }
        public bool PerfilDoadorCompleto { get; set; }
    }
}