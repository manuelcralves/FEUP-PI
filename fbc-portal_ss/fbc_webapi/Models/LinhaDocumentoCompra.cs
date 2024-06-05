using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace fbc_webapi.Models
{
    public class LinhaDocumentoCompra : LinhaDocumento
    {
        public Guid? IdCabecRasCompras { get; set; }
        public DateTime? DataEntrega { get; set; }

    }
}