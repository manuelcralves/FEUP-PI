using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace fbc_webapi.Models
{
    public class Cliente
    {
        public string Codigo { get; set; }
        public string Nome { get; set; }
        public string Morada { get; set; }
        public string CodPostal { get; set; }
        public string LocalidadeCodPostal { get; set; }
        public string Localidade { get; set; }
        public string Zona { get; set; }
        public string ZonaDescricao { get; set; }
        public string Vendedor { get; set; }
        public string CondPag { get; set; }
        public double LimiteCredito { get; set; }
        public double TotalDebito { get; set; }
        public string CodigoAPA { get; set; }
    }
}