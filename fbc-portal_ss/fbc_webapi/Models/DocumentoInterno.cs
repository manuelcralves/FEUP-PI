using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace fbc_webapi.Models
{
    public class DocumentoInterno : DocumentoBase
    {
        public List<LinhaDocumentoInterno> Linhas { get; set; }

        public DocumentoInterno()
        {
            Linhas = new List<LinhaDocumentoInterno>();
        }
    }
}