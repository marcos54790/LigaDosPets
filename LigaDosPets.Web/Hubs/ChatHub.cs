// Hubs/ChatHub.cs
using LigaDosPets.Application.Abstractions.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace LigaDosPets.Web.Hubs
{
    [Authorize]
    public class ChatHub : Hub
    {
        private readonly IConversaService _conversaService;
        private readonly IUsuarioService _usuarioService;

        public ChatHub(IConversaService conversaService, IUsuarioService usuarioService)
        {
            _conversaService = conversaService;
            _usuarioService = usuarioService;
        }

        // ── Entrar no grupo da conversa ───────────────────────
        public async Task EntrarConversa(int conversaId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"conversa-{conversaId}");
        }

        // ── Sair do grupo ─────────────────────────────────────
        public async Task SairConversa(int conversaId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"conversa-{conversaId}");
        }

        // ── Enviar mensagem ───────────────────────────────────
        public async Task EnviarMensagem(int conversaId, string conteudo)
        {
            if (string.IsNullOrWhiteSpace(conteudo)) return;

            var appUserId = Context.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(appUserId)) return;

            var perfil = await _usuarioService.GetPerfilPorApplicationUserIdAsync(appUserId);
            if (perfil == null) return;

            var mensagem = await _conversaService.EnviarMensagemAsync(conversaId, perfil.Id, conteudo.Trim());

            // Envia para todos no grupo da conversa
            await Clients.Group($"conversa-{conversaId}").SendAsync("ReceberMensagem", new
            {
                id = mensagem.Id,
                conversaId = mensagem.ConversaId,
                remetenteId = mensagem.RemetenteId,
                nomeRemetente = mensagem.NomeRemetente,
                conteudo = mensagem.Conteudo,
                dataEnvio = mensagem.DataEnvio.ToString("HH:mm"),
                meuId = perfil.Id
            });
        }

        // ── Digitar (indicador) ───────────────────────────────
        public async Task Digitando(int conversaId, bool digitando)
        {
            var appUserId = Context.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(appUserId)) return;

            var perfil = await _usuarioService.GetPerfilPorApplicationUserIdAsync(appUserId);
            if (perfil == null) return;

            await Clients.OthersInGroup($"conversa-{conversaId}").SendAsync("UsuarioDigitando", new
            {
                nome = perfil.Nome,
                digitando = digitando
            });
        }
    }
}