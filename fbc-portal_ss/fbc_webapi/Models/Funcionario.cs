using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace fbc_webapi.Models
{
    public class Funcionario
    {
        public string Codigo { get; set; }
        public string Nome { get; set; }
        public string Localidade { get; set; }
        public string Naturalidade { get; set; }
        public string Empresa { get; set; }
        public string Email { get; set; }
    }
}