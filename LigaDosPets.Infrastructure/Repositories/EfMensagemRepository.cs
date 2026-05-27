using LigaDosPets.Application.Abstractions.Repositories;
using LigaDosPets.Domain.Entities;
using LigaDosPets.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LigaDosPets.Infrastructure.Repositories
{
    public class EfMensagemRepository : Repository<Mensagem>, IMensagemRepository
    {
        public EfMensagemRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Mensagem>> GetMensagensPorConversaAsync(int conversaId)
        {
            return await _context.Mensagens
                .Where(m => m.ConversaId == conversaId)
                .Include(m => m.Remetente)
                .OrderBy(m => m.DataEnvio)
                .ToListAsync();
        }

        public async Task<int> ContarMensagensNaoLidasAsync(int conversaId, int usuarioId)
        {
            return await _context.Mensagens
                .Where(m => m.ConversaId == conversaId && !m.Lida && m.RemetenteId != usuarioId)
                .CountAsync();
        }

        public async Task MarcarComoLidasAsync(int conversaId, int usuarioId)
        {
            var mensagensNaoLidas = await _context.Mensagens
                .Where(m => m.ConversaId == conversaId && !m.Lida && m.RemetenteId != usuarioId)
                .ToListAsync();

            foreach (var msg in mensagensNaoLidas)
            {
                msg.Lida = true;
            }

            _context.Mensagens.UpdateRange(mensagensNaoLidas);
        }
    }
}