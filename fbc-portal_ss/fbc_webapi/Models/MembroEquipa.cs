using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace fbc_webapi.Models
{
    public class Membro
    {
        public bool ModoEdição { get; set; } = true;
        public Guid? Id { get; set; }   
        public string CodEquipa { get; set; }
        public string Utilizador { get; set; }
        public string Nome { get; set;}
    }
}