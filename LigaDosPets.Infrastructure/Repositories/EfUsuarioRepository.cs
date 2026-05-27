using LigaDosPets.Application.Abstractions.Repositories;
using LigaDosPets.Domain.Entities;
using LigaDosPets.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace LigaDosPets.Infrastructure.Repositories
{
    public class EfUsuarioRepository : Repository<Usuario>, IUsuarioRepository
    {
        public EfUsuarioRepository(AppDbContext context) : base(context) { }

        public async Task<Usuario?> ObterPorEmailAsync(string email)
        {
            return await _context.Usuarios
                .Include(u => u.TipoUsuario)
                .FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower());
        }

        public async Task<Usuario?> ObterPorApplicationUserIdAsync(string applicationUserId)
        {
            return await _context.Usuarios
                .Include(u => u.TipoUsuario)
                .FirstOrDefaultAsync(u => u.ApplicationUserId == applicationUserId);
        }

        public async Task<Usuario?> ObterUsuarioCompletoAsync(int id)
        {
            return await _context.Usuarios
                .Include(u => u.TipoUsuario)
                .Include(u => u.AnimaisCadastrados)
                .Include(u => u.Agendamentos)
                .Include(u => u.Adocoes)
                .FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<IEnumerable<Usuario>> ObterAdministradoresAsync()
        {
            return await _context.Usuarios
                .Where(u => u.TipoUsuario.Nome == "Administrador")
                .Include(u => u.TipoUsuario)
                .ToListAsync();
        }

        public async Task<bool> EmailExisteAsync(string email)
        {
            return await _context.Usuarios
                .AnyAsync(u => u.Email.ToLower() == email.ToLower());
        }
    }
}