namespace LigaDosPets.Application.Abstractions.Identity
{
    public interface IIdentityService
    {
        /// <summary>Cria o ApplicationUser e adiciona à role. Retorna o Id gerado.</summary>
        Task<string> CreateUserAsync(string email, string senha, string role);

        /// <summary>
        /// Remove todas as roles atuais do usuário e adiciona a nova.
        /// applicationUserId = o Id do AspNetUsers.
        /// </summary>
        Task AlterarRoleAsync(string applicationUserId, string novaRole);
    }
}