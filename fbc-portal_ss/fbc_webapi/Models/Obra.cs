using System;

namespace fbc_webapi.Models
{
    public class Obra
    {
        public Guid? Id { get; set; } 
        public string Codigo { get; set; }
        public string Descricao { get; set; }
    }
}