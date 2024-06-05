using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace fbc_webapi.Models
{
    public class MoradaFornecedor : BaseMorada
    {
        public string CodMoradaFornecedor { get; set; }
        public string CodigoAPA { get; set; }
    }
}