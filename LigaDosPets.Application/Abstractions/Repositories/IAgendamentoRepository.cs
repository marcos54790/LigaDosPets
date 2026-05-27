// Application/Abstractions/Repositories/IAgendamentoRepository.cs
using LigaDosPets.Domain.Entities;

namespace LigaDosPets.Application.Abstractions.Repositories
{
    public interface IAgendamentoRepository : IRepository<Agendamento>
    {
        Task<IEnumerable<Agendamento>> GetAgendamentosPorUsuarioAsync(int usuarioId);
        Task<IEnumerable<Agendamento>> GetAgendamentosPorAnimalAsync(int animalId);
        Task<IEnumerable<Agendamento>> GetAgendamentosPorStatusAsync(int statusId);
        Task<IEnumerable<Agendamento>> GetAgendamentosFuturosAsync();
        Task<Agendamento?> GetAgendamentoCompletoAsync(int id);

        /// <summary>
        /// Verifica se já existe um agendamento ativo (Pendente ou Confirmado)
        /// no mesmo horário — independente do animal. Controle de fluxo da ONG.
        /// </summary>
        Task<bool> HorarioOcupadoAsync(DateTime dataHora);

        /// <summary>
        /// Retorna todos os horários ocupados em uma data específica.
        /// Usado para bloquear opções no modal de agendamento.
        /// </summary>
        Task<IEnumerable<DateTime>> GetHorariosOcupadosNoDiaAsync(DateTime data);
    }
}