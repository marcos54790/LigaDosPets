using LigaDosPets.Application.Abstractions;
using LigaDosPets.Application.Abstractions.Repositories;
using LigaDosPets.Application.Abstractions.Services;
using LigaDosPets.Application.DTOs.Notificacao;
using LigaDosPets.Domain.Entities;

namespace LigaDosPets.Application.Services
{
    public class NotificacaoService : INotificacaoService
    {
        private readonly INotificacaoRepository _notificacaoRepository;
        private readonly IUnitOfWork _unitOfWork;

        public NotificacaoService(INotificacaoRepository notificacaoRepository, IUnitOfWork unitOfWork)
        {
            _notificacaoRepository = notificacaoRepository;
            _unitOfWork = unitOfWork;
        }

        private static NotificacaoDto MapToDto(Notificacao n) => new()
        {
            Id = n.Id,
            Tipo = n.Tipo,
            Titulo = n.Titulo,
            Mensagem = n.Mensagem,
            Lida = n.Lida,
            UrlAcao = n.UrlAcao,
            DataCriacao = n.DataCriacao
        };

        public async Task<IEnumerable<NotificacaoDto>> GetNotificacoesUsuarioAsync(int usuarioId)
        {
            var list = await _notificacaoRepository.GetNotificacoesPorUsuarioAsync(usuarioId);
            return list.Select(MapToDto);
        }

        public async Task<NotificacaoDto?> MarcarComoLidaAsync(int notificacaoId)
        {
            await _notificacaoRepository.MarcarComoLidaAsync(notificacaoId);
            await _unitOfWork.SaveChangesAsync();
            var n = await _notificacaoRepository.GetByIdAsync(notificacaoId);
            return n == null ? null : MapToDto(n);
        }

        public async Task MarcarTodasComoLidasAsync(int usuarioId)
        {
            await _notificacaoRepository.MarcarTodasComoLidasAsync(usuarioId);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<int> ContarNaoLidasAsync(int usuarioId)
        {
            return await _notificacaoRepository.ContarNaoLidasAsync(usuarioId);
        }

        public async Task NotificarAsync(int usuarioId, string titulo, string mensagem, string? urlAcao)
        {
            var notificacao = new Notificacao
            {
                UsuarioId = usuarioId,
                Tipo = "Geral",
                Titulo = titulo,
                Mensagem = mensagem,
                UrlAcao = urlAcao,
                DataCriacao = DateTime.UtcNow
            };

            await _notificacaoRepository.AddAsync(notificacao);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}