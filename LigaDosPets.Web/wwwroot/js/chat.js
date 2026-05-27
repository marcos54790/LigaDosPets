// Sistema de Chat em Tempo Real — Liga dos Pets (SignalR)

const ChatLiga = (function () {
    'use strict';

    let connection = null;
    let config = {};

    function el(id) { return document.getElementById(id); }

    const getMessagesArea = () => el('chatMessages');
    const getInputMensagem = () => el('inputMensagem');
    const getBtnEnviar = () => el('btnEnviar');
    const getDigitando = () => el('digitandoIndicador');

    function renderMensagem(msg) {
        const area = getMessagesArea();
        if (!area) return;

        const isMeu = msg.remetenteId === config.usuarioId || msg.meuId === config.usuarioId;
        const div = document.createElement('div');
        div.className = `chat-msg ${isMeu ? 'meu' : 'deles'}`;

        const avatarHtml = !isMeu
            ? `<div class="chat-msg-avatar"><i class="bi bi-headset"></i></div>`
            : '';

        div.innerHTML = `
            ${avatarHtml}
            <div class="chat-msg-bubble">
                <div class="chat-msg-text">${escapeHtml(msg.conteudo)}</div>
                <div class="chat-msg-time">${msg.dataEnvio}</div>
            </div>`;

        area.appendChild(div);
        scrollToBottom();
    }

    function scrollToBottom() {
        const area = getMessagesArea();
        if (area) area.scrollTop = area.scrollHeight;
    }

    function escapeHtml(str) {
        return String(str)
            .replace(/&/g, '&amp;')
            .replace(/</g, '&lt;')
            .replace(/>/g, '&gt;')
            .replace(/"/g, '&quot;');
    }

    async function enviar() {
        const input = getInputMensagem();
        if (!input) return;

        const texto = input.value.trim();
        if (!texto || !connection) return;

        try {
            await connection.invoke('EnviarMensagem', config.conversaId, texto);
            input.value = '';
            // para de digitar
            await connection.invoke('Digitando', config.conversaId, false);
        } catch (e) {
            console.error('Erro ao enviar mensagem:', e);
        }
    }

    async function init(cfg) {
        config = cfg;

        connection = new signalR.HubConnectionBuilder()
            .withUrl('/chatHub')
            .withAutomaticReconnect()
            .build();

        connection.on('ReceberMensagem', function (msg) {
            renderMensagem(msg);
        });

        connection.on('UsuarioDigitando', function (data) {
            const ind = getDigitando();
            if (!ind) return;
            if (data.digitando) {
                ind.classList.remove('d-none');
                scrollToBottom();
            } else {
                ind.classList.add('d-none');
            }
        });

        try {
            await connection.start();
            await connection.invoke('EntrarConversa', config.conversaId);
            scrollToBottom();
        } catch (e) {
            console.error('Erro ao conectar ao chat:', e);
        }

        const btn = getBtnEnviar();
        if (btn) btn.addEventListener('click', enviar);

        const input = getInputMensagem();
        if (input) {
            input.addEventListener('keydown', function (e) {
                if (e.key === 'Enter' && !e.shiftKey) {
                    e.preventDefault();
                    enviar();
                }
            });

            // Indicador de digitando
            input.addEventListener('input', function () {
                if (connection && connection.state === signalR.HubConnectionState.Connected) {
                    const estaDigitando = this.value.length > 0;
                    connection.invoke('Digitando', config.conversaId, estaDigitando).catch(() => { });
                }
            });
        }
    }

    return { init };
})();