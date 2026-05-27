using LigaDosPets.Application.Abstractions.Repositories;
using LigaDosPets.Domain.Entities;
using LigaDosPets.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LigaDosPets.Infrastructure.Repositories
{
    public class EfCaracteristicaRepository : Repository<Caracteristica>, ICaracteristicaRepository
    {
        public EfCaracteristicaRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Caracteristica>> GetCaracteristicasPorAnimalAsync(int animalId)
        {
            return await _context.AnimaisCaracteristicas
                .Where(ac => ac.AnimalId == animalId)
                .Select(ac => ac.Caracteristica)
                .ToListAsync();
        }

        public async Task<bool> ExistePorNomeAsync(string nome)
        {
            return await _context.Caracteristicas
                .AnyAsync(c => c.Nome.ToLower() == nome.ToLower());
        }
    }
}