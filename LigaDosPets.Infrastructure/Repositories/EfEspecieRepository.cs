using LigaDosPets.Application.Abstractions.Repositories;
using LigaDosPets.Domain.Entities;
using LigaDosPets.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace LigaDosPets.Infrastructure.Repositories
{
    public class EfEspecieRepository : Repository<Especie>, IEspecieRepository
    {
        public EfEspecieRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<Especie?> GetEspecieComRacasAsync(int id)
        {
            return await _context.Especies
                .Include(e => e.Racas)
                .FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task<bool> ExistePorNomeAsync(string nome)
        {
            return await _context.Especies
                .AnyAsync(e => e.Nome.ToLower() == nome.ToLower());
        }
    }
}