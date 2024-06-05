using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace fbc_webapi.Models
{
    public class TipoDocumentosVendas: TipoDocumentos
    {
        internal byte? TipoTipoDocumento { get; set; }
        internal bool? DocumentoNaoValorizado { get; set; }
    }
}