// wwwroot/js/catalogo-filtros.js
// Filtros client-side do catálogo de animais — Liga dos Pets

(function () {
    'use strict';

    const items = document.querySelectorAll('.catalog-item');
    const pills = document.querySelectorAll('.filter-pill');
    const countEl = document.getElementById('countVisible');
    const emptyEl = document.getElementById('emptyFilter');

    let activeEspecie = 'todos';
    let activeSexo = null;

    function applyFilters() {
        let visible = 0;

        items.forEach(function (item) {
            const especie = item.dataset.especie;
            const sexo = item.dataset.sexo;

            const matchEspecie = activeEspecie === 'todos' || especie === activeEspecie;
            const matchSexo = !activeSexo || sexo === activeSexo;

            if (matchEspecie && matchSexo) {
                item.classList.remove('hidden');
                visible++;
            } else {
                item.classList.add('hidden');
            }
        });

        if (countEl) countEl.textContent = visible;
        if (emptyEl) emptyEl.style.display = visible === 0 ? 'block' : 'none';
    }

    pills.forEach(function (pill) {
        pill.addEventListener('click', function () {
            if (this.dataset.filter !== undefined) {
                // Filtro de espécie
                pills.forEach(function (p) {
                    if (p.dataset.filter !== undefined) p.classList.remove('active');
                });
                this.classList.add('active');
                activeEspecie = this.dataset.filter;

            } else if (this.dataset.sexo !== undefined) {
                // Filtro de sexo — toggle
                if (this.classList.contains('active')) {
                    this.classList.remove('active');
                    activeSexo = null;
                } else {
                    pills.forEach(function (p) {
                        if (p.dataset.sexo !== undefined) p.classList.remove('active');
                    });
                    this.classList.add('active');
                    activeSexo = this.dataset.sexo;
                }
            }

            applyFilters();
        });
    });

    // Expõe para o botão inline "veja todos" no estado vazio
    window.resetFiltros = function () {
        activeEspecie = 'todos';
        activeSexo = null;
        pills.forEach(function (p) { p.classList.remove('active'); });
        var todosBtn = document.querySelector('[data-filter="todos"]');
        if (todosBtn) todosBtn.classList.add('active');
        applyFilters();
    };

})();