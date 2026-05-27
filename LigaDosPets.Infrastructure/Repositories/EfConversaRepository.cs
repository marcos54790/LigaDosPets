using LigaDosPets.Application.Abstractions.Repositories;
using LigaDosPets.Domain.Entities;
using LigaDosPets.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LigaDosPets.Infrastructure.Repositories
{
    public class EfConversaRepository : Repository<Conversa>, IConversaRepository
    {
        public EfConversaRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<Conversa?> ObterConversaEntreUsuariosAsync(int usuarioAId, int usuarioBId, int? animalId)
        {
            return await _context.Conversas
                .Include(c => c.UsuarioA)
                .Include(c => c.UsuarioB)
                .Include(c => c.Animal)
                .FirstOrDefaultAsync(c =>
                    ((c.UsuarioAId == usuarioAId && c.UsuarioBId == usuarioBId) ||
                     (c.UsuarioAId == usuarioBId && c.UsuarioBId == usuarioAId)) &&
                    c.AnimalId == animalId);
        }

        public async Task<IEnumerable<Conversa>> GetConversasPorUsuarioAsync(int usuarioId)
        {
            return await _context.Conversas
                .Where(c => c.UsuarioAId == usuarioId || c.UsuarioBId == usuarioId)
                .Include(c => c.UsuarioA)
                .Include(c => c.UsuarioB)
                .Include(c => c.Animal)
                .OrderByDescending(c => c.UltimaMensagemEm)
                .ToListAsync();
        }

        public async Task<Conversa?> GetConversaComMensagensAsync(int conversaId)
        {
            return await _context.Conversas
                .Include(c => c.Mensagens.OrderByDescending(m => m.DataEnvio))
                .FirstOrDefaultAsync(c => c.Id == conversaId);
        }

        public async Task AtualizarUltimaMensagemAsync(int conversaId, DateTime data)
        {
            var conversa = await _context.Conversas.FindAsync(conversaId);
            if (conversa != null)
            {
                conversa.UltimaMensagemEm = data;
                _context.Conversas.Update(conversa);
            }
        }
    }
}