using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FI.AtividadeEntrevista.BLL
{
    public class BoCliente
    {
        /// <summary>
        /// Inclui um novo cliente
        /// </summary>
        /// <param name="cliente">Objeto de cliente</param>
        public long Incluir(DML.Cliente cliente)
        {
            DAL.DaoCliente cli = new DAL.DaoCliente();
            return cli.Incluir(cliente);
        }

        /// <summary>
        /// Altera um cliente
        /// </summary>
        /// <param name="cliente">Objeto de cliente</param>
        public void Alterar(DML.Cliente cliente)
        {
            DAL.DaoCliente cli = new DAL.DaoCliente();
            cli.Alterar(cliente);
        }

        /// <summary>
        /// Consulta o cliente pelo id
        /// </summary>
        /// <param name="id">id do cliente</param>
        /// <returns></returns>
        public DML.Cliente Consultar(long id)
        {
            DAL.DaoCliente cli = new DAL.DaoCliente();
            return cli.Consultar(id);
        }

        /// <summary>
        /// Excluir o cliente pelo id
        /// </summary>
        /// <param name="id">id do cliente</param>
        /// <returns></returns>
        public void Excluir(long id)
        {
            DAL.DaoCliente cli = new DAL.DaoCliente();
            cli.Excluir(id);
        }

        /// <summary>
        /// Lista os clientes
        /// </summary>
        public List<DML.Cliente> Listar()
        {
            DAL.DaoCliente cli = new DAL.DaoCliente();
            return cli.Listar();
        }

        /// <summary>
        /// Lista os clientes
        /// </summary>
        public List<DML.Cliente> Pesquisa(int iniciarEm, int quantidade, string campoOrdenacao, bool crescente, out int qtd)
        {
            DAL.DaoCliente cli = new DAL.DaoCliente();
            return cli.Pesquisa(iniciarEm,  quantidade, campoOrdenacao, crescente, out qtd);
        }

        /// <summary>
        /// VerificaExistencia
        /// </summary>
        /// <param name="CPF"></param>
        /// <returns></returns>
        public bool VerificarExistencia(string CPF)
        {
            DAL.DaoCliente cli = new DAL.DaoCliente();
            return cli.VerificarExistencia(CPF);
        }

        /// <summary>
        /// verifica se é um CPF Válido.
        /// </summary>
        public bool IsCpf(string CPF)
        {
            bool isValid = true;
            if (CPF.Length != 11) return false;

            // Verifica se todos os caracteres dígitos.
            for (int i = 0; i < CPF.Length; i++)
            {
                if (!char.IsDigit(CPF[i]))
                {
                    isValid = false;
                    break;
                }
            }

            // verifica se todos os digitos são iguais. (00000000000, ..., 99999999999)
            if (isValid)
            {
                for (byte i = 0; i < 10; i++)
                {
                    var temp = new string(Convert.ToChar(i), 11);
                    if (CPF == temp)
                    {
                        isValid = false;
                        break;
                    }
                }

            }

            // Verifica o dígito de controle.
            if (isValid)
            {
                var j = 0;
                var d1 = 0;
                var d2 = 0;

                // Primeiro digito de controle.
                for (int i = 10; i > 1; i--)
                {
                    d1 += Convert.ToInt32(CPF.Substring(j, 1)) * i;
                    j++;
                }

                d1 = (d1 * 10) % 11;
                if (d1 == 10) d1 = 0;

                // verifica se d1 = ao CPF enviado pelo usuário.
                if (d1 != Convert.ToInt32(CPF.Substring(9, 1))) isValid = false;

                // Segundo digito de controle.
                if (isValid)
                {
                    j = 0;
                    for (int i = 11; i > 1; i--)
                    {
                        d2 += Convert.ToInt32(CPF.Substring(j, 1)) * i;
                        j++;
                    }

                    d2 = (d2 * 10) % 11;
                    if (d2 == 10) d2 = 0;

                    // verifica se d2 = ao CPF enviado pelo usuário.
                    if (d2 != Convert.ToInt32(CPF.Substring(10, 1))) isValid = false;
                }
            }

            return isValid;
        }
    }
}
