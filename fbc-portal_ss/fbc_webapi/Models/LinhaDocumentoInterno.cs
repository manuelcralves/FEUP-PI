using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace fbc_webapi.Models
{
    public class LinhaDocumentoInterno : LinhaDocumento
    {
        public Guid? IdCabecRasInternos { get; set; }
        public DateTime? DataEntrega { get; set; }

    }
}