using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace fbc_webapi.Models
{
    public class Serie
    {
        public string Codigo { get; set; }
        public string Descricao { get; set; }
        public bool SeriePorDefeito { get; set; }
    }
}