(function () {
    'use strict';

    var path = window.location.pathname.toLowerCase();
    if (path.includes('/admin') || path.includes('/auth') || path.includes('/chat')) return;

    var btn = document.createElement('a');
    btn.href = '/Chat/Iniciar';
    btn.className = 'chat-float-btn';
    btn.title = 'Falar com o suporte';
    btn.innerHTML = '<i class="bi bi-chat-dots-fill"></i><span>Suporte</span>';

    document.body.appendChild(btn);

    setTimeout(function () {
        btn.classList.add('visible');
    }, 1000);
})();