/**
 * Gerenciador de Notificações e Scripts de Formulário - Liga dos Pets
 */
const NotificacaoManager = {
    exibirDetalhes: function (titulo, mensagem, tipo) {
        const iconDiv = document.getElementById('notifIcon');
        const titleEl = document.getElementById('notifTitle');
        const msgEl = document.getElementById('notifMessage');
        const modalEl = document.getElementById('modalNotificacao');

        if (!modalEl) return;

        titleEl.innerText = titulo;
        msgEl.innerText = mensagem;

        if (tipo === 'success') {
            iconDiv.innerHTML = '<i class="bi bi-check-circle-fill text-success display-1"></i>';
        } else if (tipo === 'danger') {
            iconDiv.innerHTML = '<i class="bi bi-exclamation-triangle-fill text-danger display-1"></i>';
        } else {
            iconDiv.innerHTML = '<i class="bi bi-info-circle-fill text-primary display-1"></i>';
        }

        const myModal = new bootstrap.Modal(modalEl);
        myModal.show();
    },

    limparAlerta: function () {
        const dot = document.querySelector('.notification-dot');
        if (dot) dot.style.display = 'none';
    }
};

// Lógica para carregar raças dinamicamente
document.addEventListener('DOMContentLoaded', function () {
    const selectEspecie = document.getElementById('selectEspecie');
    const selectRaca = document.getElementById('selectRaca');

    if (selectEspecie) {
        selectEspecie.addEventListener('change', async function () {
            const id = this.value;

            if (!id) {
                selectRaca.disabled = true;
                selectRaca.innerHTML = '<option value="">Aguardando espécie...</option>';
                return;
            }

            try {
                const response = await fetch(`/Dashboard/ObterRacasPorEspecie?especieId=${id}`);
                const racas = await response.json();

                selectRaca.innerHTML = '<option value="">Selecione a Raça</option>';
                racas.forEach(r => {
                    const opt = document.createElement('option');
                    opt.value = r.id;
                    opt.textContent = r.nome;
                    selectRaca.appendChild(opt);
                });

                selectRaca.disabled = false;
            } catch (e) {
                console.error("Erro ao buscar raças", e);
            }
        });
    }
});