using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace fbc_webapi.Models
{
    public class Email
    {
        public string Utilizador { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public string CC { get; set; }
        public string BCC { get; set; }
        public string Assunto { get; set; }
        public string Mensagem { get; set; }
        public string Anexos { get; set; }
        public bool Formato { get; set; }
    }
}