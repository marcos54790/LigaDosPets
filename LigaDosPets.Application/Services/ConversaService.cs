using LigaDosPets.Application.Abstractions;
using LigaDosPets.Application.Abstractions.Repositories;
using LigaDosPets.Application.Abstractions.Services;
using LigaDosPets.Application.DTOs.Conversa;
using LigaDosPets.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LigaDosPets.Application.Services
{
    public class ConversaService : IConversaService
    {
        private readonly IConversaRepository _conversaRepository;
        private readonly IMensagemRepository _mensagemRepository;
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly INotificacaoService _notificacaoService;

        public ConversaService(
            IConversaRepository conversaRepository,
            IMensagemRepository mensagemRepository,
            IUsuarioRepository usuarioRepository,
            IUnitOfWork unitOfWork,
            INotificacaoService notificacaoService)
        {
            _conversaRepository = conversaRepository;
            _mensagemRepository = mensagemRepository;
            _usuarioRepository = usuarioRepository;
            _unitOfWork = unitOfWork;
            _notificacaoService = notificacaoService;
        }

        private static MensagemDto MapMensagemToDto(Mensagem m)
        {
            return new MensagemDto
            {
                Id = m.Id,
                ConversaId = m.ConversaId,
                RemetenteId = m.RemetenteId,
                NomeRemetente = m.Remetente?.Nome ?? string.Empty,
                Conteudo = m.Conteudo,
                Lida = m.Lida,
                DataEnvio = m.DataEnvio
            };
        }

        private static ConversaDto MapConversaToDto(Conversa c, int usuarioLogadoId)
        {
            var outroUsuario = c.UsuarioAId == usuarioLogadoId ? c.UsuarioB : c.UsuarioA;
            return new ConversaDto
            {
                Id = c.Id,
                UsuarioAId = c.UsuarioAId,
                UsuarioBId = c.UsuarioBId,
                NomeOutroUsuario = outroUsuario?.Nome ?? string.Empty,
                AnimalId = c.AnimalId,
                NomeAnimal = c.Animal?.Nome,
                UltimaMensagemEm = c.UltimaMensagemEm
            };
        }

        public async Task<ConversaDto> IniciarOuObterConversaAsync(int usuarioLogadoId, int outroUsuarioId, int? animalId)
        {
            var existente = await _conversaRepository.ObterConversaEntreUsuariosAsync(usuarioLogadoId, outroUsuarioId, animalId);
            if (existente != null)
                return MapConversaToDto(existente, usuarioLogadoId);

            var nova = new Conversa
            {
                UsuarioAId = usuarioLogadoId,
                UsuarioBId = outroUsuarioId,
                AnimalId = animalId,
                DataCriacao = DateTime.UtcNow
            };

            await _conversaRepository.AddAsync(nova);
            await _unitOfWork.SaveChangesAsync();

            return MapConversaToDto(nova, usuarioLogadoId);
        }

        public async Task<MensagemDto> EnviarMensagemAsync(int conversaId, int remetenteId, string conteudo)
        {
            var conversa = await _conversaRepository.GetByIdAsync(conversaId);
            if (conversa == null) throw new InvalidOperationException("Conversa não encontrada.");

            var mensagem = new Mensagem
            {
                ConversaId = conversaId,
                RemetenteId = remetenteId,
                Conteudo = conteudo,
                Lida = false,
                DataEnvio = DateTime.UtcNow
            };

            await _mensagemRepository.AddAsync(mensagem);
            conversa.UltimaMensagemEm = mensagem.DataEnvio;
            await _conversaRepository.UpdateAsync(conversa);
            await _unitOfWork.SaveChangesAsync();

            // Notificar o outro participante
            int destinatarioId = conversa.UsuarioAId == remetenteId ? conversa.UsuarioBId : conversa.UsuarioAId;
            await _notificacaoService.NotificarAsync(
                destinatarioId,
                "Nova mensagem",
                "Você recebeu uma nova mensagem no chat.",
                $"/Chat/Conversa/{conversaId}"
            );

            var mensagemCompleta = await _mensagemRepository.Query()
                .Include(m => m.Remetente)
                .FirstOrDefaultAsync(m => m.Id == mensagem.Id);
            return MapMensagemToDto(mensagemCompleta!);
        }

        public async Task<IEnumerable<MensagemDto>> GetMensagensConversaAsync(int conversaId, int usuarioSolicitanteId)
        {
            var mensagens = await _mensagemRepository.GetMensagensPorConversaAsync(conversaId);
            await _mensagemRepository.MarcarComoLidasAsync(conversaId, usuarioSolicitanteId);
            await _unitOfWork.SaveChangesAsync();
            return mensagens.Select(MapMensagemToDto);
        }

        public async Task<IEnumerable<ConversaResumoDto>> GetConversasUsuarioAsync(int usuarioId)
        {
            var conversas = await _conversaRepository.GetConversasPorUsuarioAsync(usuarioId);
            var resumos = new List<ConversaResumoDto>();

            foreach (var c in conversas)
            {
                var outroUsuario = c.UsuarioAId == usuarioId ? c.UsuarioB : c.UsuarioA;
                var ultimaMensagem = c.Mensagens?.OrderByDescending(m => m.DataEnvio).FirstOrDefault();
                int naoLidas = await _mensagemRepository.ContarMensagensNaoLidasAsync(c.Id, usuarioId);

                resumos.Add(new ConversaResumoDto
                {
                    Id = c.Id,
                    NomeOutroUsuario = outroUsuario?.Nome ?? string.Empty,
                    NomeAnimal = c.Animal?.Nome,
                    UltimaMensagem = ultimaMensagem?.Conteudo,
                    UltimaMensagemEm = c.UltimaMensagemEm,
                    NaoLidas = naoLidas
                });
            }

            return resumos;
        }

        public async Task MarcarMensagensComoLidasAsync(int conversaId, int usuarioId)
        {
            await _mensagemRepository.MarcarComoLidasAsync(conversaId, usuarioId);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task ExcluirConversaAsync(int conversaId)
        {
            var conversa = await _conversaRepository.GetByIdAsync(conversaId);
            if (conversa == null) throw new InvalidOperationException("Conversa não encontrada.");

            var mensagens = await _mensagemRepository.GetMensagensPorConversaAsync(conversaId);
            foreach (var msg in mensagens)
                await _mensagemRepository.DeleteAsync(msg);

            await _conversaRepository.DeleteAsync(conversa);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}