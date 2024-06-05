using Microsoft.Extensions.DependencyModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace fbc_webapi.Models
{
    public class LinhaDocumentoVenda : LinhaDocumento
    {
        public float? Desconto { get; set; }
        public DateTime? DataEntrega { get;  set; }
    }
}