// wwwroot/js/auth-login.js
// Scripts da página de Login — Liga dos Pets

(function () {
    'use strict';

    function togglePwd(inputId, btn) {
        var input = document.getElementById(inputId);
        var icon = btn.querySelector('i');
        if (input.type === 'password') {
            input.type = 'text';
            icon.className = 'bi bi-eye-slash';
        } else {
            input.type = 'password';
            icon.className = 'bi bi-eye';
        }
    }

    // Expõe para uso inline nos botões do HTML
    window.togglePwd = togglePwd;

})();