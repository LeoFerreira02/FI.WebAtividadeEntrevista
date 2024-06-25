using System.Collections.Generic;
using FI.AtividadeEntrevista.DAL;

namespace FI.AtividadeEntrevista.BLL
{
    public class BoBene
    {
        /// <summary>
        /// Inclui novos beneficiários para um cliente
        /// </summary>
        /// <param name="idCliente">ID do cliente ao qual os beneficiários serão associados</param>
        /// <param name="beneficiarios">Lista de beneficiários a serem incluídos</param>
        public void IncluirBeneficiarios(long idCliente, IEnumerable<DML.Bene> beneficiarios)
        {
            DaoBene daoBene = new DaoBene();
            daoBene.IncluirBeneficiarios(idCliente, beneficiarios);
        }

        /// <summary>
        /// Exclui todos os beneficiários de um cliente
        /// </summary>
        /// <param name="idCliente">ID do cliente cujos beneficiários serão excluídos</param>
        public void ExcluirBeneficiarios(long idCliente)
        {
            DaoBene daoBene = new DaoBene();
            daoBene.ExcluirBeneficiarios(idCliente);
        }
    }
}