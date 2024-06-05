using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace fbc_webapi.Models
{
    public class BaseMorada
    {
        public string Morada { get; set; }
        public string Localidade { get; set; }
        public string CodigoPostal { get; set; }
        public string Zona { get; set; }
        public string ZonaDescricao { get; internal set; }
    }
}