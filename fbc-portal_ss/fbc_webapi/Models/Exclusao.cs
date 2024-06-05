using System;

namespace fbc_webapi.Models
{
    public class Exclusao
    {
        public Guid? Id { get; set; }
        public string IdCabecDoc { get; set; }
        public string Codigo { get; set; }
        public string Motivo { get; set; }
    }
}