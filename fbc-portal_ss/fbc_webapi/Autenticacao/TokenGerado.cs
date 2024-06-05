using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace fbc_webapi.Autenticacao
{
    public class TokenGerado
    {
        public Guid IdToken { get; set; }
        public string Username { get; set; }
        public byte[] PasswordEncriptada { get; set; }
        public DateTimeOffset? DataUtcGerado { get; set; }
        public DateTimeOffset? DataUtcExpiracao { get; set; }
    }
}