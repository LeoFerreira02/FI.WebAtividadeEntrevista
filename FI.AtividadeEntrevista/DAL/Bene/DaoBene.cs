using FI.AtividadeEntrevista.DML;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace FI.AtividadeEntrevista.DAL
{
    internal class DaoBene : AcessoDados
    {
        internal void IncluirBeneficiarios(long idCliente, IEnumerable<Bene> beneficiarios)
        {
            foreach (var beneficiario in beneficiarios)
            {
                List<System.Data.SqlClient.SqlParameter> parametros = new List<System.Data.SqlClient.SqlParameter>();

                parametros.Add(new System.Data.SqlClient.SqlParameter("IdCliente", idCliente));
                parametros.Add(new System.Data.SqlClient.SqlParameter("Nome", beneficiario.Nome));
                parametros.Add(new System.Data.SqlClient.SqlParameter("CPF", beneficiario.CPF));

                Executar("FI_SP_IncBene", parametros);
            }
        }

        internal List<Bene> ListarBeneficiarios(long idCliente)
        {
            List<System.Data.SqlClient.SqlParameter> parametros = new List<System.Data.SqlClient.SqlParameter>();

            parametros.Add(new System.Data.SqlClient.SqlParameter("IdCliente", idCliente));

            DataSet ds = Consultar("FI_SP_ListBene", parametros);
            List<Bene> beneficiarios = ConverterBeneficiarios(ds);

            return beneficiarios;
        }

        internal void AlterarBeneficiarios(long idCliente, IEnumerable<Bene> beneficiarios)
        {
            // Lógica para atualizar beneficiários (Não desenvolvi ainda porque não consegui recuperar os dados da lista de beneficiários)
        }

        internal void ExcluirBeneficiarios(long idCliente)
        {
            List<System.Data.SqlClient.SqlParameter> parametros = new List<System.Data.SqlClient.SqlParameter>();

            parametros.Add(new System.Data.SqlClient.SqlParameter("IdCliente", idCliente));

            Executar("FI_SP_DelBene", parametros);
        }

        private List<Bene> ConverterBeneficiarios(DataSet ds)
        {
            List<Bene> lista = new List<Bene>();
            if (ds != null && ds.Tables != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    Bene beneficiario = new Bene();
                    beneficiario.Id = row.Field<long>("Id");
                    beneficiario.IdCliente = row.Field<long>("IdCliente");
                    beneficiario.Nome = row.Field<string>("Nome");
                    beneficiario.CPF = row.Field<string>("CPF");
                    lista.Add(beneficiario);
                }
            }

            return lista;
        }
    }
}
