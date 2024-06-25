using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebAtividadeEntrevista.Models
{
    /// <summary>
    /// Classe de Modelo de Cliente
    /// </summary>
    public class BeneModel
    {
        public long Id { get; set; }

        public string Nome { get; set; }

        public string CPF { get; set; }
    }
}