using LigaDosPets.Application.Abstractions.Repositories;
using LigaDosPets.Domain.Entities;
using LigaDosPets.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace LigaDosPets.Infrastructure.Repositories
{
    public class EfStatusSolicitacaoRepository : Repository<StatusSolicitacao>, IStatusSolicitacaoRepository
    {
        public EfStatusSolicitacaoRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<StatusSolicitacao?> ObterPorNomeAsync(string nome)
        {
            return await _context.StatusSolicitacoes
                .FirstOrDefaultAsync(s => s.Nome.ToLower() == nome.ToLower());
        }
    }
}