// wwwroot/js/auth-register.js
// Scripts da página de Cadastro — Liga dos Pets

(function() {
    'use strict';

    // ── Toggle visibilidade da senha ──────────────────────────
    function togglePwd(inputId, btn)
    {
        var input = document.getElementById(inputId);
        var icon = btn.querySelector('i');
        if (input.type === 'password')
        {
            input.type = 'text';
            icon.className = 'bi bi-eye-slash';
        }
        else
        {
            input.type = 'password';
            icon.className = 'bi bi-eye';
        }
    }

    window.togglePwd = togglePwd;

    // ── Indicador de força da senha ───────────────────────────
    var senhaInput = document.getElementById('senhaInput');
    var confirmarInput = document.getElementById('confirmarSenhaInput');
    var strengthBar = document.getElementById('strengthBar');
    var strengthFill = document.getElementById('strengthFill');
    var strengthLabel = document.getElementById('strengthLabel');
    var mismatch = document.getElementById('senhaMismatch');

    var levels = [
        { color: '#ef4444', label: 'Muito fraca',  width: '20%'  },
        { color: '#f97316', label: 'Fraca',        width: '40%'  },
        { color: '#eab308', label: 'Média',        width: '60%'  },
        { color: '#22c55e', label: 'Forte',        width: '80%'  },
        { color: '#16a34a', label: 'Muito forte',  width: '100%' },
    ];

    function calcStrength(pwd)
    {
        var score = 0;
        if (pwd.length >= 6) score++;
        if (pwd.length >= 10) score++;
        if (/ [A - Z] /.test(pwd)) score++;
        if (/ [0 - 9] /.test(pwd)) score++;
        if (/ [^A - Za - z0 - 9] /.test(pwd)) score++;
        return Math.min(score, 4);
    }

    if (senhaInput)
    {
        senhaInput.addEventListener('input', function() {
            var val = this.value;
            if (!val)
            {
                strengthBar.style.display = 'none';
                return;
            }
            strengthBar.style.display = 'flex';
            var lvl = levels[calcStrength(val)];
            strengthFill.style.width = lvl.width;
            strengthFill.style.background = lvl.color;
            strengthLabel.textContent = lvl.label;
            strengthLabel.style.color = lvl.color;
        });
    }

    // ── Validação de confirmação de senha ─────────────────────
    if (confirmarInput)
    {
        confirmarInput.addEventListener('input', function() {
            if (this.value && this.value !== senhaInput.value)
            {
                mismatch.style.display = 'block';
            }
            else
            {
                mismatch.style.display = 'none';
            }
        });
    }

})();