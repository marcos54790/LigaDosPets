using LigaDosPets.Application.Abstractions.Repositories;
using LigaDosPets.Domain.Entities;
using LigaDosPets.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LigaDosPets.Infrastructure.Repositories
{
    public class EfAdocaoRepository : Repository<Adocao>, IAdocaoRepository
    {
        public EfAdocaoRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<Adocao?> GetAdocaoComRelacionamentosAsync(int id)
        {
            return await _context.Adocoes
                .Include(a => a.Animal)
                .Include(a => a.UsuarioAdotante)
                .Include(a => a.AdminConfirmou)
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<IEnumerable<Adocao>> GetAdocoesPorAnimalAsync(int animalId)
        {
            return await _context.Adocoes
                .Where(a => a.AnimalId == animalId)
                .Include(a => a.UsuarioAdotante)
                .ToListAsync();
        }

        public async Task<IEnumerable<Adocao>> GetAdocoesPorAdotanteAsync(int usuarioId)
        {
            return await _context.Adocoes
                .Where(a => a.UsuarioAdotanteId == usuarioId)
                .Include(a => a.Animal)
                .ToListAsync();
        }

        public async Task<bool> AnimalJaFoiAdotadoAsync(int animalId)
        {
            return await _context.Adocoes.AnyAsync(a => a.AnimalId == animalId);
        }
    }
}