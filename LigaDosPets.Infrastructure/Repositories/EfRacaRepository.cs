using LigaDosPets.Application.Abstractions.Repositories;
using LigaDosPets.Domain.Entities;
using LigaDosPets.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LigaDosPets.Infrastructure.Repositories
{
    public class EfRacaRepository : Repository<Raca>, IRacaRepository
    {
        public EfRacaRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Raca>> GetRacasPorEspecieAsync(int especieId)
        {
            return await _context.Racas
                .Where(r => r.EspecieId == especieId)
                .ToListAsync();
        }

        public async Task<bool> ExistePorNomeEEspecieAsync(string nome, int especieId)
        {
            return await _context.Racas
                .AnyAsync(r => r.Nome.ToLower() == nome.ToLower() && r.EspecieId == especieId);
        }
    }
}