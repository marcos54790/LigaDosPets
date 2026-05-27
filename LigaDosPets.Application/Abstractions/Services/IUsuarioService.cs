using LigaDosPets.Application.DTOs.Usuario;

namespace LigaDosPets.Application.Abstractions.Services
{
    public interface IUsuarioService
    {
        Task<UsuarioPerfilDto?> GetPerfilAsync(int id);
        Task<UsuarioPerfilDto?> GetPerfilPorApplicationUserIdAsync(string applicationUserId);
        Task<UsuarioPerfilDto> AtualizarPerfilAsync(int id, UpdateUsuarioDto dto);
        Task AtualizarDadosDoadorAsync(string applicationUserId, CompletarPerfilDto dto);
        Task<IEnumerable<UsuarioListDto>> GetAllAsync();
        Task<bool> EmailExisteAsync(string email);
        Task<UsuarioPerfilDto> RegistrarUsuarioExternoAsync(RegistroUsuarioDto dto);
        Task<UsuarioPerfilDto> CriarUsuarioAsync(RegistroUsuarioDto dto, int tipoUsuarioId);
        Task DesativarUsuarioAsync(int id);
        Task AtivarUsuarioAsync(int id);
        Task ExcluirUsuarioAsync(int id);
        Task<UsuarioPerfilDto?> GetAdminPrincipalAsync();

        /// <summary>
        /// Altera a role do usuário no Identity e o TipoUsuario no domínio.
        /// novaRole: "Usuário" | "Gerente" | "Administrador"
        /// </summary>
        Task PromoverUsuarioAsync(int id, string novaRole);
    }
}