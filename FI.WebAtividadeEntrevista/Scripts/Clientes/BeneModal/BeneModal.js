$(document).ready(function () {

    $('#ModalCPF').mask('000.000.000-00');

    $('#btnIncluir').click(function () {
        var cpf = $('#ModalCPF').val().trim();
        var nome = $('#ModalNome').val().trim();

        if (cpf === '' || nome === '') {
            alert('Por favor, preencha todos os campos.');
            return;
        }

        if (!validaCPF(cpf)) {
            alert('CPF inválido.');
            return;
        }

        // Verifica se o CPF já existe na tabela
        if (cpfJaExisteNaTabela(cpf)) {
            alert('CPF já cadastrado na tabela.');
            return;
        }

        var novaLinha = adicionarBeneficiario(cpf, nome);
        $('#tabelaBeneficiarios tbody').append(novaLinha);

        $('#ModalCPF').val('');
        $('#ModalNome').val('');
    });

    function adicionarBeneficiario(cpf, nome) {
        var idx = $('#tabelaBeneficiarios tbody tr').length;

        var novaLinha = '<tr>' +
            '<td style="display: none;">' +
            '<input name="Beneficiarios[' + idx + '].ID" value="0"/>' +
            '<input name="Beneficiarios[' + idx + '].CPF" value="' + cpf + '"/>' +
            '<input name="Beneficiarios[' + idx + '].Nome" value="' + nome + '"/>' +
            '</td>' +
            '<td>' + cpf + '</td>' +
            '<td>' + nome + '</td>' +
            '<td class="text-right">' +
            '<button type="button" class="btn btn-primary btn-alterar">Alterar</button>' +
            '&nbsp;' +
            '&nbsp;' +
            '<button type="button" class="btn btn-primary btn-excluir">Excluir</button>' +
            '</td>' +
            '</tr>';

        return novaLinha;
    }

    // Função para verificar se o CPF já existe na tabela
    function cpfJaExisteNaTabela(cpf) {
        var existe = false;
        $('#tabelaBeneficiarios tbody').find('tr').each(function () {
            // idx nao comeca do (0) porque ao criar os dados dinamicamente, o campo de CPF passa a ser o idx(1).
            var cpfNaTabela = $(this).find('td:eq(1)').text().trim();
            if (cpfNaTabela === cpf) {
                existe = true;
                return false;
            }
        });
        return existe;
    }

    // Função para excluir uma linha da tabela de beneficiários
    $('#tabelaBeneficiarios').on('click', '.btn-excluir', function () {
        $(this).closest('tr').remove();
    });

    // Preenchendo e abrindo a modal de edição.
    $('#tabelaBeneficiarios').on('click', '.btn-alterar', function () {
        var row = $(this).closest('tr');
        var cpf = row.find('td:eq(1)').text().trim();
        var nome = row.find('td:eq(2)').text().trim();

        $('#EditCPF').val(cpf);
        $('#EditNome').val(nome);

        $('#modalEditar').modal('show');
    });

    // Função para salvar as alterações no modal de edição
    $('#btnSalvarAlteracoes').click(function () {
        var cpf = $('#EditCPF').val().trim();
        var nome = $('#EditNome').val().trim();

        if (cpf === '' || nome === '') {
            alert('Por favor, preencha todos os campos.');
            return;
        }

        if (!validaCPF(cpf)) {
            alert('CPF inválido.');
            return;
        }

        // Atualiza os dados na tabela de beneficiários

        // idx nao comeca do (0) porque ao criar os dados dinamicamente, o campo de CPF passa a ser o idx(1).
        $('#tabelaBeneficiarios tbody').find('tr').each(function () {
            if ($(this).find('td:eq(1)').text().trim() === cpf) {
                $(this).find('td:eq(2)').text(nome);
            }
        });

        $('#modalEditar').modal('hide');
    });

    function validaCPF(cpf) {
        cpf = cpf.replace(/[^\d]+/g, ''); // Remove caracteres não numéricos
        if (cpf === '') return false;

        // verifica se todos os digitos são iguais. (00000000000, ..., 99999999999)
        var primeiroDigito = cpf.charAt(0);
        var todosIguais = true;
        for (var i = 0; i < cpf.length; i++) {
            if (cpf.charAt(i) !== primeiroDigito) {
                todosIguais = false;
                break;
            }
        }
        if (todosIguais) {
            return false;
        }

        // Primeiro digito de controle.
        var add = 0;
        for (var i = 0; i < 9; i++)
            add += parseInt(cpf.charAt(i)) * (10 - i);
        var rev = 11 - (add % 11);
        if (rev === 10 || rev === 11)
            rev = 0;
        if (rev !== parseInt(cpf.charAt(9)))
            return false;

        // Segundo digito de controle.
        add = 0;
        for (var i = 0; i < 10; i++)
            add += parseInt(cpf.charAt(i)) * (11 - i);
        rev = 11 - (add % 11);
        if (rev === 10 || rev === 11)
            rev = 0;
        if (rev !== parseInt(cpf.charAt(10)))
            return false;

        return true;
    }
});