using LigaDosPets.Application.Abstractions.Repositories;
using LigaDosPets.Domain.Entities;
using LigaDosPets.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LigaDosPets.Infrastructure.Repositories
{
    public class EfFotoRepository : Repository<Foto>, IFotoRepository
    {
        public EfFotoRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Foto>> GetFotosPorAnimalAsync(int animalId)
        {
            return await _context.Fotos
                .Where(f => f.AnimalId == animalId)
                .OrderByDescending(f => f.IsPrincipal)
                .ToListAsync();
        }

        public async Task<Foto?> GetFotoPrincipalAsync(int animalId)
        {
            return await _context.Fotos
                .FirstOrDefaultAsync(f => f.AnimalId == animalId && f.IsPrincipal);
        }

        public async Task DefinirFotoPrincipalAsync(int fotoId)
        {
            var foto = await _context.Fotos.FindAsync(fotoId);
            if (foto != null)
            {
                var fotosDoAnimal = await _context.Fotos
                    .Where(f => f.AnimalId == foto.AnimalId)
                    .ToListAsync();

                foreach (var f in fotosDoAnimal)
                {
                    f.IsPrincipal = (f.Id == fotoId);
                }

                _context.Fotos.UpdateRange(fotosDoAnimal);
            }
        }
    }
}