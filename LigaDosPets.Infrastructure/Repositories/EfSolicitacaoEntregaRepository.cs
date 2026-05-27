using LigaDosPets.Application.Abstractions.Repositories;
using LigaDosPets.Domain.Entities;
using LigaDosPets.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LigaDosPets.Infrastructure.Repositories
{
    public class EfSolicitacaoEntregaRepository : Repository<SolicitacaoEntrega>, ISolicitacaoEntregaRepository
    {
        public EfSolicitacaoEntregaRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<SolicitacaoEntrega>> GetSolicitacoesPendentesAsync()
        {
            return await _context.SolicitacoesEntregas
                .Where(s => s.StatusId == 1) // 1 = Pendente
                .Include(s => s.Usuario)
                .Include(s => s.Status)
                .OrderByDescending(s => s.DataSolicitacao)
                .ToListAsync();
        }

        public async Task<IEnumerable<SolicitacaoEntrega>> GetSolicitacoesPorUsuarioAsync(int usuarioId)
        {
            return await _context.SolicitacoesEntregas
                .Where(s => s.UsuarioId == usuarioId)
                .Include(s => s.Status)
                .Include(s => s.Animal)
                .OrderByDescending(s => s.DataSolicitacao)
                .ToListAsync();
        }

        public async Task<IEnumerable<SolicitacaoEntrega>> GetSolicitacoesPorStatusAsync(int statusId)
        {
            return await _context.SolicitacoesEntregas
                .Where(s => s.StatusId == statusId)
                .Include(s => s.Usuario)
                .Include(s => s.Status)
                .ToListAsync();
        }

        public async Task<SolicitacaoEntrega?> GetSolicitacaoCompletaAsync(int id)
        {
            return await _context.SolicitacoesEntregas
                .Include(s => s.Usuario)
                .Include(s => s.Status)
                .Include(s => s.Animal)
                .FirstOrDefaultAsync(s => s.Id == id);
        }
    }
}