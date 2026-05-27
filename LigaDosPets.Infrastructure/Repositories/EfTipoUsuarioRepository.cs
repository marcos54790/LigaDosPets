using LigaDosPets.Application.Abstractions.Repositories;
using LigaDosPets.Domain.Entities;
using LigaDosPets.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace LigaDosPets.Infrastructure.Repositories
{
    public class EfTipoUsuarioRepository : Repository<TipoUsuario>, ITipoUsuarioRepository
    {
        public EfTipoUsuarioRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<TipoUsuario?> ObterPorNomeAsync(string nome)
        {
            return await _context.TiposUsuarios
                .FirstOrDefaultAsync(t => t.Nome.ToLower() == nome.ToLower());
        }
    }
}