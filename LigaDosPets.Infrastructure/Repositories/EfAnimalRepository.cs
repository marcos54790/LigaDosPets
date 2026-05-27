using LigaDosPets.Application.Abstractions.Repositories;
using LigaDosPets.Domain.Entities;
using LigaDosPets.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace LigaDosPets.Infrastructure.Repositories
{
    public class EfAnimalRepository : Repository<Animal>, IAnimalRepository
    {
        public EfAnimalRepository(AppDbContext context) : base(context)
        {
        }

        // Implementação necessária para resolver o erro CS1061 no AnimalService
        public async Task<IEnumerable<Animal>> GetByExpressionAsync(Expression<Func<Animal, bool>> expression)
        {
            return await _context.Animais
                .Where(expression)
                .Include(a => a.Porte)
                .Include(a => a.Especie)
                .Include(a => a.Raca)
                .Include(a => a.Fotos)
                .ToListAsync();
        }

        public async Task<IEnumerable<Animal>> GetDisponiveisParaAdocaoAsync()
        {
            return await _context.Animais
                .Where(a => a.DisponivelParaAdocao && !a.AguardandoValidacao)
                .Include(a => a.Porte)
                .Include(a => a.Especie)
                .Include(a => a.Raca)
                .Include(a => a.Fotos)
                .ToListAsync();
        }

        public async Task<IEnumerable<Animal>> GetPendentesValidacaoAsync()
        {
            return await _context.Animais
                .Where(a => a.AguardandoValidacao)
                .Include(a => a.Porte)
                .Include(a => a.Especie)
                .Include(a => a.Raca)
                .Include(a => a.UsuarioCadastro)
                .ToListAsync();
        }

        public async Task<Animal?> GetAnimalComDetalhesAsync(int id)
        {
            return await _context.Animais
                .Include(a => a.Porte)
                .Include(a => a.Especie)
                .Include(a => a.Raca)
                .Include(a => a.Fotos)
                .Include(a => a.UsuarioCadastro)
                .Include(a => a.AnimalCaracteristicas)
                    .ThenInclude(ac => ac.Caracteristica)
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<IEnumerable<Animal>> GetAnimaisPorUsuarioAsync(int usuarioId)
        {
            return await _context.Animais
                .Where(a => a.UsuarioCadastroId == usuarioId)
                .Include(a => a.Porte)
                .Include(a => a.Especie)
                .Include(a => a.Raca)
                .Include(a => a.Fotos)
                .ToListAsync();
        }
    }
}