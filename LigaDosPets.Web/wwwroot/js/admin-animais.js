// wwwroot/js/admin-animais.js
// Scripts do CRUD de Animais — Painel Admin — Liga dos Pets

(function () {
    'use strict';

    document.addEventListener('DOMContentLoaded', function () {

        // ── Modal: Rejeitar ───────────────────────────────────
        var modalRejeitar = document.getElementById('modalRejeitar');
        if (modalRejeitar) {
            modalRejeitar.addEventListener('show.bs.modal', function (event) {
                var btn = event.relatedTarget;
                var id = btn.dataset.id;
                var nome = btn.dataset.nome;

                document.getElementById('nomeAnimalRejeitar').textContent = nome;
                document.getElementById('formRejeitar').action =
                    '/Admin/Animais/Rejeitar/' + id;
            });
        }

        // ── Modal: Excluir ────────────────────────────────────
        var modalExcluir = document.getElementById('modalExcluir');
        if (modalExcluir) {
            modalExcluir.addEventListener('show.bs.modal', function (event) {
                var btn = event.relatedTarget;
                var id = btn.dataset.id;
                var nome = btn.dataset.nome;

                document.getElementById('nomeAnimalExcluir').textContent = nome;
                document.getElementById('formExcluir').action =
                    '/Admin/Animais/Delete/' + id;
            });
        }

    });

})();