using Microsoft.AspNetCore.Identity;
using LigaDosPets.Domain.Entities;

namespace LigaDosPets.Infrastructure.Identity
{
    public class ApplicationUser : IdentityUser
    {
        // Relacionamento 1:1 com Usuario (domínio)
        public virtual Usuario? Usuario { get; set; }
    }
}