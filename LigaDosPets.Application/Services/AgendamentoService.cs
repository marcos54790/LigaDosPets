// Application/Services/AgendamentoService.cs
using LigaDosPets.Application.Abstractions;
using LigaDosPets.Application.Abstractions.Repositories;
using LigaDosPets.Application.Abstractions.Services;
using LigaDosPets.Application.DTOs.Agendamento;
using LigaDosPets.Domain.Entities;

namespace LigaDosPets.Application.Services
{
    public class AgendamentoService : IAgendamentoService
    {
        private readonly IAgendamentoRepository _agendamentoRepository;
        private readonly IStatusAgendamentoRepository _statusRepository;
        private readonly IAnimalRepository _animalRepository;
        private readonly IUnitOfWork _unitOfWork;

        public AgendamentoService(
            IAgendamentoRepository agendamentoRepository,
            IStatusAgendamentoRepository statusRepository,
            IAnimalRepository animalRepository,
            IUnitOfWork unitOfWork)
        {
            _agendamentoRepository = agendamentoRepository;
            _statusRepository = statusRepository;
            _animalRepository = animalRepository;
            _unitOfWork = unitOfWork;
        }

        private static AgendamentoDto MapToDto(Agendamento agendamento) => new()
        {
            Id = agendamento.Id,
            AnimalId = agendamento.AnimalId,
            NomeAnimal = agendamento.Animal?.Nome ?? string.Empty,
            UsuarioId = agendamento.UsuarioId,
            NomeUsuario = agendamento.Usuario?.Nome ?? string.Empty,
            StatusAgendamentoId = agendamento.StatusAgendamentoId,
            Status = agendamento.StatusAgendamento?.Nome ?? string.Empty,
            DataVisita = agendamento.DataVisita,
            Observacao = agendamento.Observacao,
            DataCriacao = agendamento.DataCriacao
        };

        public async Task<AgendamentoDto> CriarAgendamentoAsync(CreateAgendamentoDto dto, int usuarioId)
        {
            var animal = await _animalRepository.GetByIdAsync(dto.AnimalId);
            if (animal == null || !animal.DisponivelParaAdocao)
                throw new InvalidOperationException("Animal não disponível para visita.");

            // ── Validação de horário permitido ──────────────────────
            var hora = dto.DataVisita.Hour;
            var minuto = dto.DataVisita.Minute;
            if (minuto != 0 || hora < 9 || hora >= 17 || (hora >= 12 && hora < 14))
                throw new InvalidOperationException(
                    "Horário inválido. Os agendamentos são de segunda a sexta, das 9h às 12h e das 14h às 17h, em intervalos de 1 hora.");

            // ── Validação de conflito global de horário ─────────────
            var horarioOcupado = await _agendamentoRepository.HorarioOcupadoAsync(dto.DataVisita);
            if (horarioOcupado)
                throw new InvalidOperationException(
                    $"O horário {dto.DataVisita:HH:mm} do dia {dto.DataVisita:dd/MM/yyyy} já está reservado.");

            var statusPendente = await _statusRepository.ObterPorNomeAsync("Pendente");
            if (statusPendente == null)
                throw new InvalidOperationException("Status Pendente não encontrado.");

            var agendamento = new Agendamento
            {
                AnimalId = dto.AnimalId,
                UsuarioId = usuarioId,
                StatusAgendamentoId = statusPendente.Id,
                DataVisita = dto.DataVisita,
                Observacao = dto.Observacao,
                DataCriacao = DateTime.UtcNow
            };

            await _agendamentoRepository.AddAsync(agendamento);
            await _unitOfWork.SaveChangesAsync();

            var completo = await _agendamentoRepository.GetAgendamentoCompletoAsync(agendamento.Id);
            return MapToDto(completo!);
        }

        public async Task<AgendamentoDto?> GetByIdAsync(int id)
        {
            var agendamento = await _agendamentoRepository.GetAgendamentoCompletoAsync(id);
            return agendamento == null ? null : MapToDto(agendamento);
        }

        public async Task<IEnumerable<AgendamentoDto>> GetAgendamentosUsuarioAsync(int usuarioId)
        {
            var agendamentos = await _agendamentoRepository.GetAgendamentosPorUsuarioAsync(usuarioId);
            return agendamentos.Select(MapToDto);
        }

        public async Task<IEnumerable<AgendamentoDto>> GetAgendamentosPendentesAsync()
        {
            var statusPendente = await _statusRepository.ObterPorNomeAsync("Pendente");
            if (statusPendente == null) return Enumerable.Empty<AgendamentoDto>();
            var agendamentos = await _agendamentoRepository.GetAgendamentosPorStatusAsync(statusPendente.Id);
            return agendamentos.Select(MapToDto);
        }

        public async Task<IEnumerable<AgendamentoDto>> GetAgendamentosFuturosAsync()
        {
            var agendamentos = await _agendamentoRepository.GetAgendamentosFuturosAsync();
            return agendamentos
                .Where(a => a.StatusAgendamento?.Nome != "Cancelado" &&
                            a.StatusAgendamento?.Nome != "Realizado")
                .Select(MapToDto);
        }

        public async Task<AgendamentoDto> ConfirmarAgendamentoAsync(int id, int adminId)
        {
            var agendamento = await _agendamentoRepository.GetByIdAsync(id);
            if (agendamento == null) throw new InvalidOperationException("Agendamento nao encontrado.");

            var statusConfirmado = await _statusRepository.ObterPorNomeAsync("Confirmado");
            if (statusConfirmado == null) throw new InvalidOperationException("Status Confirmado nao encontrado.");

            agendamento.StatusAgendamentoId = statusConfirmado.Id;
            await _agendamentoRepository.UpdateAsync(agendamento);
            await _unitOfWork.SaveChangesAsync();

            var completo = await _agendamentoRepository.GetAgendamentoCompletoAsync(id);
            return MapToDto(completo!);
        }

        public async Task<AgendamentoDto> RealizarAgendamentoAsync(int id)
        {
            var agendamento = await _agendamentoRepository.GetByIdAsync(id);
            if (agendamento == null) throw new InvalidOperationException("Agendamento nao encontrado.");

            var statusRealizado = await _statusRepository.ObterPorNomeAsync("Realizado");
            if (statusRealizado == null) throw new InvalidOperationException("Status Realizado nao encontrado.");

            agendamento.StatusAgendamentoId = statusRealizado.Id;
            await _agendamentoRepository.UpdateAsync(agendamento);
            await _unitOfWork.SaveChangesAsync();

            var completo = await _agendamentoRepository.GetAgendamentoCompletoAsync(id);
            return MapToDto(completo!);
        }

        public async Task<AgendamentoDto> CancelarAgendamentoAsync(int id, string motivo, int usuarioId)
        {
            var agendamento = await _agendamentoRepository.GetByIdAsync(id);
            if (agendamento == null) throw new InvalidOperationException("Agendamento nao encontrado.");

            var statusCancelado = await _statusRepository.ObterPorNomeAsync("Cancelado");
            if (statusCancelado == null) throw new InvalidOperationException("Status Cancelado nao encontrado.");

            agendamento.StatusAgendamentoId = statusCancelado.Id;
            agendamento.Observacao = (agendamento.Observacao ?? "") + " | Cancelado: " + motivo;
            await _agendamentoRepository.UpdateAsync(agendamento);
            await _unitOfWork.SaveChangesAsync();

            var completo = await _agendamentoRepository.GetAgendamentoCompletoAsync(id);
            return MapToDto(completo!);
        }

        public async Task<IEnumerable<string>> GetHorariosOcupadosNoDiaAsync(DateTime data)
        {
            var horarios = await _agendamentoRepository.GetHorariosOcupadosNoDiaAsync(data);
            return horarios.Select(h => h.ToString("HH:mm"));
        }
    }
}