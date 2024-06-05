using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace fbc_webapi.Models
{
    public class Utilizador
    {
        public string Codigo { get; set; }
        public string Nome { get; set; }
        public string Email { get; set; }
        public bool Administrador { get; set; } 

    }
}