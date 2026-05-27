// wwwroot/js/raca-dinamica.js
// Carrega raças dinamicamente ao selecionar espécie — Liga dos Pets
// Funciona tanto no painel do usuário (/Dashboard) quanto no admin (/Admin/Animais)

(function () {
    'use strict';

    document.addEventListener('DOMContentLoaded', function () {
        var selectEspecie = document.getElementById('selectEspecie');
        var selectRaca = document.getElementById('selectRaca');

        if (!selectEspecie || !selectRaca) return;

        // ── Detecta contexto pela URL ─────────────────────────
        // Admin  → /Admin/Animais/ObterRacasPorEspecie
        // Usuário → /Dashboard/ObterRacasPorEspecie
        function getEndpointUrl(especieId) {
            var path = window.location.pathname.toLowerCase();
            if (path.includes('/admin/')) {
                return '/Admin/Animais/ObterRacasPorEspecie?especieId=' + especieId;
            }
            return '/Dashboard/ObterRacasPorEspecie?especieId=' + especieId;
        }

        selectEspecie.addEventListener('change', async function () {
            var id = this.value;

            if (!id) {
                selectRaca.disabled = true;
                selectRaca.innerHTML = '<option value="">Aguardando espécie...</option>';
                return;
            }

            selectRaca.disabled = true;
            selectRaca.innerHTML = '<option value="">Carregando...</option>';

            try {
                var response = await fetch(getEndpointUrl(id));

                if (!response.ok) {
                    throw new Error('Resposta inválida: ' + response.status);
                }

                var racas = await response.json();

                selectRaca.innerHTML = '<option value="">Selecione a raça</option>';

                racas.forEach(function (r) {
                    var opt = document.createElement('option');
                    opt.value = r.id;
                    opt.textContent = r.nome;
                    selectRaca.appendChild(opt);
                });

                selectRaca.disabled = false;

            } catch (e) {
                console.error('Erro ao buscar raças:', e);
                selectRaca.innerHTML = '<option value="">Erro ao carregar raças</option>';
                selectRaca.disabled = false;
            }
        });
    });

})();