// Infrastructure/Repositories/EfAgendamentoRepository.cs
using LigaDosPets.Application.Abstractions.Repositories;
using LigaDosPets.Domain.Entities;
using LigaDosPets.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace LigaDosPets.Infrastructure.Repositories
{
    public class EfAgendamentoRepository : Repository<Agendamento>, IAgendamentoRepository
    {
        public EfAgendamentoRepository(AppDbContext context) : base(context) { }

        public async Task<IEnumerable<Agendamento>> GetAgendamentosPorUsuarioAsync(int usuarioId)
        {
            return await _context.Agendamentos
                .Include(a => a.Animal)
                .Include(a => a.StatusAgendamento)
                .Where(a => a.UsuarioId == usuarioId)
                .OrderByDescending(a => a.DataVisita)
                .ToListAsync();
        }

        public async Task<IEnumerable<Agendamento>> GetAgendamentosPorAnimalAsync(int animalId)
        {
            return await _context.Agendamentos
                .Include(a => a.Usuario)
                .Include(a => a.StatusAgendamento)
                .Where(a => a.AnimalId == animalId)
                .OrderByDescending(a => a.DataVisita)
                .ToListAsync();
        }

        public async Task<IEnumerable<Agendamento>> GetAgendamentosPorStatusAsync(int statusId)
        {
            return await _context.Agendamentos
                .Include(a => a.Animal)
                .Include(a => a.Usuario)
                .Include(a => a.StatusAgendamento)
                .Where(a => a.StatusAgendamentoId == statusId)
                .OrderBy(a => a.DataVisita)
                .ToListAsync();
        }

        public async Task<IEnumerable<Agendamento>> GetAgendamentosFuturosAsync()
        {
            return await _context.Agendamentos
                .Include(a => a.Animal)
                .Include(a => a.Usuario)
                .Include(a => a.StatusAgendamento)
                .Where(a => a.DataVisita >= DateTime.UtcNow)
                .OrderBy(a => a.DataVisita)
                .ToListAsync();
        }

        public async Task<Agendamento?> GetAgendamentoCompletoAsync(int id)
        {
            return await _context.Agendamentos
                .Include(a => a.Animal)
                .Include(a => a.Usuario)
                .Include(a => a.StatusAgendamento)
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        /// <summary>
        /// Verifica conflito global: só um agendamento ativo por horário na ONG.
        /// Status cancelado/realizado não bloqueia o horário.
        /// </summary>
        public async Task<bool> HorarioOcupadoAsync(DateTime dataHora)
        {
            return await _context.Agendamentos
                .Include(a => a.StatusAgendamento)
                .AnyAsync(a =>
                    a.DataVisita == dataHora &&
                    a.StatusAgendamento.Nome != "Cancelado" &&
                    a.StatusAgendamento.Nome != "Realizado");
        }

        /// <summary>
        /// Retorna todos os horários ocupados no dia para bloquear no modal.
        /// </summary>
        public async Task<IEnumerable<DateTime>> GetHorariosOcupadosNoDiaAsync(DateTime data)
        {
            var inicio = data.Date;
            var fim = inicio.AddDays(1);

            return await _context.Agendamentos
                .Include(a => a.StatusAgendamento)
                .Where(a =>
                    a.DataVisita >= inicio &&
                    a.DataVisita < fim &&
                    a.StatusAgendamento.Nome != "Cancelado" &&
                    a.StatusAgendamento.Nome != "Realizado")
                .Select(a => a.DataVisita)
                .ToListAsync();
        }
    }
}