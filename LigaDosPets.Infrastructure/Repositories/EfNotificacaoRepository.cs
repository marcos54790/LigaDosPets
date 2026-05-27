using LigaDosPets.Application.Abstractions.Repositories;
using LigaDosPets.Domain.Entities;
using LigaDosPets.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LigaDosPets.Infrastructure.Repositories
{
    public class EfNotificacaoRepository : Repository<Notificacao>, INotificacaoRepository
    {
        public EfNotificacaoRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Notificacao>> GetNotificacoesPorUsuarioAsync(int usuarioId)
        {
            return await _context.Notificacoes
                .Where(n => n.UsuarioId == usuarioId)
                .OrderByDescending(n => n.DataCriacao)
                .ToListAsync();
        }

        public async Task<IEnumerable<Notificacao>> GetNotificacoesNaoLidasAsync(int usuarioId)
        {
            return await _context.Notificacoes
                .Where(n => n.UsuarioId == usuarioId && !n.Lida)
                .OrderByDescending(n => n.DataCriacao)
                .ToListAsync();
        }

        public async Task<int> ContarNaoLidasAsync(int usuarioId)
        {
            return await _context.Notificacoes
                .CountAsync(n => n.UsuarioId == usuarioId && !n.Lida);
        }

        public async Task MarcarComoLidaAsync(int id)
        {
            var notificacao = await _context.Notificacoes.FindAsync(id);
            if (notificacao != null)
            {
                notificacao.Lida = true;
                _context.Notificacoes.Update(notificacao);
            }
        }

        public async Task MarcarTodasComoLidasAsync(int usuarioId)
        {
            var naoLidas = await _context.Notificacoes
                .Where(n => n.UsuarioId == usuarioId && !n.Lida)
                .ToListAsync();

            foreach (var n in naoLidas)
            {
                n.Lida = true;
            }

            _context.Notificacoes.UpdateRange(naoLidas);
        }
    }
}