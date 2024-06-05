using System;

namespace fbc_webapi.Models
{
    public class TipoServico
    {
        public Guid? Id { get; set; }
        public string IdCabecDoc { get; set; }
        public string Codigo { get; set; }
        public string Descricao { get; set; }
    }
}