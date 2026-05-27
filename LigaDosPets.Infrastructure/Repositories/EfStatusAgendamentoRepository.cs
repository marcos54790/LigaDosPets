using LigaDosPets.Application.Abstractions.Repositories;
using LigaDosPets.Domain.Entities;
using LigaDosPets.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace LigaDosPets.Infrastructure.Repositories
{
    public class EfStatusAgendamentoRepository : Repository<StatusAgendamento>, IStatusAgendamentoRepository
    {
        public EfStatusAgendamentoRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<StatusAgendamento?> ObterPorNomeAsync(string nome)
        {
            return await _context.StatusAgendamentos
                .FirstOrDefaultAsync(s => s.Nome.ToLower() == nome.ToLower());
        }
    }
}