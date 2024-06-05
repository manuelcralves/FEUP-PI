using Microsoft.SqlServer.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace fbc_webapi.Models
{
    public class DocumentoCompra : DocumentoBase
    {
        public List<LinhaDocumentoCompra> Linhas { get; set; }

        public DocumentoCompra()
        {
            Linhas = new List<LinhaDocumentoCompra>();
        }
    }
}