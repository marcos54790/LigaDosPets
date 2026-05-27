$(document).ready(function () {
    // Aplicando Máscaras
    $('#cpf').mask('000.000.000-00');
    $('#cep').mask('00000-000');

    var maskBehavior = function (val) {
        return val.replace(/\D/g, '').length === 11 ? '(00) 00000-0000' : '(00) 0000-00009';
    },
        options = {
            onKeyPress: function (val, e, field, options) {
                field.mask(maskBehavior.apply({}, arguments), options);
            }
        };
    $('#telefone').mask(maskBehavior, options);
});

function buscarEndereco(cep) {
    const value = cep.replace(/\D/g, '');
    if (value.length !== 8) return;

    fetch(`https://viacep.com.br/ws/${value}/json/`)
        .then(res => res.json())
        .then(data => {
            if (!data.erro) {
                // Preenche campos visíveis
                document.getElementById('cidadeUf').value = `${data.localidade} - ${data.uf}`;
                document.getElementById('ruaDisplay').value = data.logradouro || "Rua não encontrada (preencha no número)";

                // Preenche campos ocultos para o C#
                document.getElementById('logradouro').value = data.logradouro || "";
                document.getElementById('bairro').value = data.bairro || "";
                document.getElementById('cidade').value = data.localidade || "";
                document.getElementById('estado').value = data.uf || "";

                const numeroInput = document.querySelector('input[name="Numero"]');
                if (numeroInput) numeroInput.focus();
            } else {
                alert("CEP não encontrado.");
            }
        })
        .catch(err => console.error("Erro ao buscar CEP:", err));
}