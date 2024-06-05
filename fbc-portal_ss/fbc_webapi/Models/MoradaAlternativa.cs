using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace fbc_webapi.Models
{
    public class MoradaAlternativa : BaseMorada
    {
        public string CodMoradaAlternativa { get; set; }
        public string CodigoAPA { get; set; }
    }
}