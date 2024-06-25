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

        internal void ExcluirBeneficiarios(long idCliente)
        {
            List<System.Data.SqlClient.SqlParameter> parametros = new List<System.Data.SqlClient.SqlParameter>();

            parametros.Add(new System.Data.SqlClient.SqlParameter("IdCliente", idCliente));

            Executar("FI_SP_DelBene", parametros);
        }
    }
}
