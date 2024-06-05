using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace fbc_webapi.Models
{
    public class SerieVendas : Serie
    {
        public string ReportPorDefeito { get; set; }
        public int? NumVias { get; set; }
        public bool? Previsualizar { get; set; }
    }
}