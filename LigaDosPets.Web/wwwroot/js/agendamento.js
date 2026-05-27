(function () {
    'use strict';

    const HORARIOS_DISPONIVEIS = ['09:00', '10:00', '11:00', '14:00', '15:00', '16:00', '17:00', '18:00'];

    const inputData = document.getElementById('inputData');
    const horariosGrid = document.getElementById('horariosGrid');
    const avisoFDS = document.getElementById('avisoFDS');
    const alertaHorario = document.getElementById('alertaHorario');
    const alertaTexto = document.getElementById('alertaHorarioTexto');
    const loadingSpinner = document.getElementById('loadingHorarios');
    const btnConfirmar = document.getElementById('btnConfirmar');

    if (!inputData || !horariosGrid) return;

    // ── Verifica dia da semana ────────────────────────────────
    function isDiaUtil(dataStr) {
        const partes = dataStr.split('-');
        const data = new Date(partes[0], partes[1] - 1, partes[2]);
        const dia = data.getDay();
        return dia !== 0 && dia !== 6;
    }

    // ── Renderiza os botões de horário ────────────────────────
    function renderHorarios(ocupados) {
        horariosGrid.innerHTML = '';

        HORARIOS_DISPONIVEIS.forEach(function (hora) {
            const ocupado = ocupados.includes(hora);

            const slot = document.createElement('div');
            slot.className = 'horario-slot' + (ocupado ? ' ocupado' : '');
            slot.dataset.hora = hora;

            const id = 'h-' + hora.replace(':', '');

            const radio = document.createElement('input');
            radio.type = 'radio';
            radio.className = 'btn-check';
            radio.name = 'horaVisita';
            radio.id = id;
            radio.value = hora;
            radio.required = true;
            if (ocupado) radio.disabled = true;

            const label = document.createElement('label');
            label.className = 'horario-label';
            label.htmlFor = id;
            label.innerHTML = hora;

            if (ocupado) {
                const badge = document.createElement('span');
                badge.className = 'horario-ocupado-badge';
                badge.textContent = 'Ocupado';
                label.appendChild(badge);
            }

            slot.appendChild(radio);
            slot.appendChild(label);
            horariosGrid.appendChild(slot);
        });
    }

    // ── Busca horários ocupados no servidor ───────────────────
    async function carregarHorariosOcupados(dataStr) {
        if (loadingSpinner) loadingSpinner.classList.remove('d-none');
        if (btnConfirmar) btnConfirmar.disabled = true;

        try {
            const resp = await fetch('/Details/HorariosOcupados?data=' + dataStr);
            const ocupados = await resp.json();
            renderHorarios(ocupados);

            // Avisa se todos os horários estão ocupados
            const todosOcupados = ocupados.length >= HORARIOS_DISPONIVEIS.length;
            if (alertaHorario && alertaTexto) {
                if (todosOcupados) {
                    alertaTexto.textContent = 'Todos os horários deste dia estão reservados. Escolha outra data.';
                    alertaHorario.classList.remove('d-none');
                } else {
                    alertaHorario.classList.add('d-none');
                }
            }
        } catch (err) {
            console.error('Erro ao carregar horários:', err);
            renderHorarios([]); // fallback: mostra todos disponíveis
        } finally {
            if (loadingSpinner) loadingSpinner.classList.add('d-none');
            if (btnConfirmar) btnConfirmar.disabled = false;
        }
    }

    // ── Listener de mudança de data ───────────────────────────
    inputData.addEventListener('change', function () {
        const val = this.value;
        if (!val) return;

        if (!isDiaUtil(val)) {
            if (avisoFDS) avisoFDS.style.display = 'block';
            this.value = '';
            renderHorarios([]); // reseta os botões
            return;
        }

        if (avisoFDS) avisoFDS.style.display = 'none';
        carregarHorariosOcupados(val);
    });

    // ── Validação antes de submeter ───────────────────────────
    const form = document.getElementById('formAgendamento');
    if (form) {
        form.addEventListener('submit', function (e) {
            const horarioSelecionado = form.querySelector('input[name="horaVisita"]:checked');
            if (!horarioSelecionado) {
                e.preventDefault();
                if (alertaHorario && alertaTexto) {
                    alertaTexto.textContent = 'Selecione um horário disponível antes de confirmar.';
                    alertaHorario.classList.remove('d-none');
                }
                return;
            }
            // Verifica se o horário selecionado não foi ocupado entre o carregamento e o submit
            const slot = horariosGrid.querySelector('[data-hora="' + horarioSelecionado.value + '"]');
            if (slot && slot.classList.contains('ocupado')) {
                e.preventDefault();
                if (alertaHorario && alertaTexto) {
                    alertaTexto.textContent = 'Este horário foi reservado por outro usuário. Escolha outro.';
                    alertaHorario.classList.remove('d-none');
                }
            }
        });
    }

})();