using System;

namespace fbc_webapi.Autenticacao
{
    public class TokenAcessoFicheiro
    {
        public Guid IdTokenGeradoAutenticacao { get; set; }
        public string FilePath { get; set; }
        public string FileName { get; set; }
        public DateTimeOffset DataUtcExpiracao { get; set; }
    }
}