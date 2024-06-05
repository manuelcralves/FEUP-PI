using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace fbc_webapi.Models
{
    public class Artigo
    {
        public string Codigo { get; set; }
        public string Marca { get; set; }
        public string Modelo { get; set; }
        public string CodBarras { get; set; }
        public string Descricao { get; set; }
        public string UnidadeVenda { get; set; }
        public double? PVP1 { get; set; }
    }
}