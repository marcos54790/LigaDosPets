using LigaDosPets.Application.Abstractions.Repositories;
using LigaDosPets.Domain.Entities;
using LigaDosPets.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace LigaDosPets.Infrastructure.Repositories
{
    public class EfPorteRepository : Repository<Porte>, IPorteRepository
    {
        public EfPorteRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<bool> ExistePorNomeAsync(string nome)
        {
            return await _context.Portes
                .AnyAsync(p => p.Nome.ToLower() == nome.ToLower());
        }
    }
}