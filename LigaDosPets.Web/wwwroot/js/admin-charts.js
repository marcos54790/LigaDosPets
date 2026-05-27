// wwwroot/js/admin-charts.js
// Gráficos do painel administrativo — Liga dos Pets
// Depende de: Chart.js (carregado no _AdminLayout)

(function () {
    'use strict';

    const PALETTE = {
        especies: [
            '#1D4ED8', '#F97316', '#16a34a', '#7c3aed',
            '#dc2626', '#0891b2', '#d97706', '#db2777'
        ],
        portes: ['#bfdbfe', '#3b82f6', '#1e3a8a'],
        sexo: ['#3b82f6', '#f472b6'],
        linha: '#1D4ED8',
    };

    const FONT = "'Poppins', sans-serif";

    Chart.defaults.font.family = FONT;
    Chart.defaults.color = '#64748b';

    // ── Helpers ──────────────────────────────────────────────
    function getData() {
        // Lê do <script id="dashboardData" type="application/json">
        const el = document.getElementById('dashboardData');
        if (!el) return null;
        try {
            return JSON.parse(el.textContent);
        } catch (e) {
            console.error('Erro ao parsear dados do dashboard:', e);
            return null;
        }
    }

    function getCanvas(id) {
        return document.getElementById(id);
    }

    function showEmpty(canvasId, msg) {
        const canvas = getCanvas(canvasId);
        if (!canvas) return;
        const wrapper = canvas.parentNode;
        canvas.remove();
        const p = document.createElement('p');
        p.className = 'text-muted text-center py-4';
        p.style.fontSize = '0.85rem';
        p.textContent = msg || 'Sem dados para exibir.';
        wrapper.appendChild(p);
    }

    const doughnutOptions = {
        responsive: true,
        maintainAspectRatio: false,
        cutout: '62%',
        plugins: {
            legend: {
                position: 'bottom',
                labels: { padding: 14, font: { size: 12 }, boxWidth: 12 }
            },
            tooltip: { padding: 10 }
        },
        animation: { animateScale: true, duration: 600 }
    };

    // ── 1. Gráfico: Espécies (doughnut) ──────────────────────
    function initEspecies(data) {
        if (!data.especiesValores || !data.especiesValores.length) {
            showEmpty('chartEspecies', 'Nenhum animal cadastrado.');
            return;
        }
        new Chart(getCanvas('chartEspecies'), {
            type: 'doughnut',
            data: {
                labels: data.especiesLabels,
                datasets: [{
                    data: data.especiesValores,
                    backgroundColor: PALETTE.especies,
                    borderWidth: 2,
                    borderColor: '#fff',
                    hoverOffset: 6
                }]
            },
            options: doughnutOptions
        });
    }

    // ── 2. Gráfico: Sexo (doughnut pequeno) ──────────────────
    function initSexo(data) {
        const total = (data.sexoMachos || 0) + (data.sexoFemeas || 0);
        if (total === 0) {
            showEmpty('chartSexo', 'Sem dados.');
            return;
        }
        new Chart(getCanvas('chartSexo'), {
            type: 'doughnut',
            data: {
                labels: ['Machos', 'Fêmeas'],
                datasets: [{
                    data: [data.sexoMachos, data.sexoFemeas],
                    backgroundColor: PALETTE.sexo,
                    borderWidth: 2,
                    borderColor: '#fff',
                    hoverOffset: 4
                }]
            },
            options: {
                ...doughnutOptions,
                plugins: {
                    ...doughnutOptions.plugins,
                    legend: {
                        position: 'bottom',
                        labels: { padding: 10, font: { size: 11 }, boxWidth: 10 }
                    }
                }
            }
        });
    }

    // ── 3. Gráfico: Portes (bar horizontal) ──────────────────
    function initPortes(data) {
        if (!data.portesValores || !data.portesValores.some(v => v > 0)) {
            showEmpty('chartPortes', 'Nenhum animal cadastrado.');
            return;
        }
        new Chart(getCanvas('chartPortes'), {
            type: 'bar',
            data: {
                labels: data.portesLabels,
                datasets: [{
                    label: 'Animais',
                    data: data.portesValores,
                    backgroundColor: PALETTE.portes,
                    borderRadius: 8,
                    borderSkipped: false,
                    maxBarThickness: 48
                }]
            },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                indexAxis: 'y',
                plugins: {
                    legend: { display: false },
                    tooltip: { padding: 10 }
                },
                scales: {
                    x: {
                        grid: { color: '#f1f5f9' },
                        ticks: { precision: 0 }
                    },
                    y: {
                        grid: { display: false }
                    }
                },
                animation: { duration: 600 }
            }
        });
    }

    // ── 4. Gráfico: Cadastros por mês (line) ─────────────────
    function initMeses(data) {
        if (!data.mesesLabels || !data.mesesLabels.length) {
            showEmpty('chartMeses', 'Sem dados de cadastro.');
            return;
        }
        new Chart(getCanvas('chartMeses'), {
            type: 'line',
            data: {
                labels: data.mesesLabels,
                datasets: [{
                    label: 'Cadastros',
                    data: data.mesesValores,
                    borderColor: PALETTE.linha,
                    backgroundColor: 'rgba(29, 78, 216, 0.08)',
                    borderWidth: 2.5,
                    pointBackgroundColor: PALETTE.linha,
                    pointRadius: 5,
                    pointHoverRadius: 7,
                    fill: true,
                    tension: 0.4
                }]
            },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                plugins: {
                    legend: { display: false },
                    tooltip: { padding: 10 }
                },
                scales: {
                    x: { grid: { display: false } },
                    y: {
                        grid: { color: '#f1f5f9' },
                        ticks: { precision: 0 },
                        beginAtZero: true
                    }
                },
                animation: { duration: 700 }
            }
        });
    }

    // ── Init ──────────────────────────────────────────────────
    document.addEventListener('DOMContentLoaded', function () {
        const data = getData();
        if (!data) {
            console.warn('dashboardData não encontrado.');
            return;
        }
        initEspecies(data);
        initSexo(data);
        initPortes(data);
        initMeses(data);
    });

})();