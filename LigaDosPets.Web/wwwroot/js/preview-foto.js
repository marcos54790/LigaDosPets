// wwwroot/js/preview-foto.js
// Preview de fotos antes do upload — Liga dos Pets

(function () {
    'use strict';

    window.previewImages = function (input) {
        var container = document.getElementById('previewContainer');
        if (!container) return;

        container.innerHTML = '';

        var files = Array.from(input.files).slice(0, 5); // máximo 5

        files.forEach(function (file) {
            if (!file.type.startsWith('image/')) return;

            var reader = new FileReader();
            reader.onload = function (e) {
                var img = document.createElement('img');
                img.src = e.target.result;
                img.alt = file.name;
                container.appendChild(img);
            };
            reader.readAsDataURL(file);
        });

        // Avisa se mais de 5 foram selecionadas
        if (input.files.length > 5) {
            var aviso = document.createElement('p');
            aviso.className = 'text-warning small mt-1 w-100';
            aviso.textContent = 'Apenas as primeiras 5 fotos serão enviadas.';
            container.appendChild(aviso);
        }
    };

})();